using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    public class NumberEntity : TableEntity
    {
        public NumberEntity(string key, string sum)
        {
            this.PartitionKey = key;
            this.RowKey = sum;
        }

        public NumberEntity() { }
    }
}