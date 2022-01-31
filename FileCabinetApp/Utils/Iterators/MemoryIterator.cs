using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.Iterators
{
    internal class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;
        private int currentPosition;

        public MemoryIterator(IEnumerable<FileCabinetRecord> records)
        {
            this.records = new List<FileCabinetRecord>(records);
        }

        public FileCabinetRecord GetNext()
        {
            return this.records[this.currentPosition++];
        }

        public bool HasMore()
        {
            return this.currentPosition < this.records.Count;
        }
    }
}
