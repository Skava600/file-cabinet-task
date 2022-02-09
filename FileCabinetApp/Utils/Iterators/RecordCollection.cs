using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Services;

namespace FileCabinetApp.Utils.Iterators
{
    /// <summary>
    /// Record iterator.
    /// </summary>
    internal class RecordCollection : IEnumerable<FileCabinetRecord>, IDisposable
    {
        private readonly List<long> offsets;
        private readonly BinaryReader binaryReader;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordCollection"/> class.
        /// </summary>
        /// <param name="fileStream"> File stream. </param>
        /// <param name="offsets"> Offsets of records in filestream. </param>
        public RecordCollection(FileStream fileStream, IEnumerable<long> offsets)
        {
            this.binaryReader = new BinaryReader(fileStream, Encoding.Unicode, true);
            this.offsets = new List<long>(offsets);
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var offset in this.offsets)
            {
                this.binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                yield return FileCabinetFilesystemService.ReadRecordFromStream(this.binaryReader);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources of RecordCollection.
        /// </summary>
        /// <param name="disposing"> disposing or not. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.binaryReader.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
