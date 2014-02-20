using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WorkerRole1 entry point called", "Information");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("people");
            table.CreateIfNotExists();

            int key = 0;
            while (true)
            {
                CloudQueueMessage peekedMessage = queue.PeekMessage();
                if (peekedMessage == null)
                {
                    continue;
                }
                string numbers = peekedMessage.AsString;
                char[] delimiterChars = { ' ' };
                string[] nums = numbers.Split(delimiterChars);
                int sum = 0;
                queue.DeleteMessage(peekedMessage);
                foreach (string number in nums) {
                    sum += Convert.ToInt32(number);
                }
                key++;
                NumberEntity entry = new NumberEntity(Convert.ToString(key), Convert.ToString(sum));
                TableOperation insertOperation = TableOperation.Insert(entry);
                table.Execute(insertOperation);

                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
