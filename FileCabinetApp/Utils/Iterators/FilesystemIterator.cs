using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Services;

namespace FileCabinetApp.Utils.Iterators
{
    internal class FilesystemIterator : IRecordIterator
    {
        private readonly List<long> offsets;
        private readonly FileStream fileStream;
        private int currentPosition;

        public FilesystemIterator(FileStream fileStream, IEnumerable<long> offsets)
        {
            this.fileStream = fileStream;
            this.offsets = new List<long>(offsets);
            this.currentPosition = 0;
        }

        public FileCabinetRecord GetNext()
        {
            if (!this.HasMore())
            {
                throw new ArgumentOutOfRangeException();
            }

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(this.offsets[this.currentPosition++], SeekOrigin.Begin);
                return FileCabinetFilesystemService.ReadRecordFromStream(binaryReader);
            }
        }

        public bool HasMore()
        {
            return this.currentPosition < this.offsets.Count;
        }
    }
}
