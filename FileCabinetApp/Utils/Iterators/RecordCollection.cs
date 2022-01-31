using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.Iterators
{
    internal class RecordCollection : IEnumerable<FileCabinetRecord>
    {
        private readonly List<long> offsets;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordCollection"/> class.
        /// </summary>
        /// <param name="fileStream"> File stream</param>
        /// <param name="offsets"> Offsets of records in filestream</param>
        public RecordCollection(FileStream fileStream, IEnumerable<long> offsets)
        {
            this.fileStream = fileStream;
            this.offsets = new List<long>(offsets);
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemIterator(
                new BinaryReader(this.fileStream, Encoding.Unicode, true),
                this.offsets);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
