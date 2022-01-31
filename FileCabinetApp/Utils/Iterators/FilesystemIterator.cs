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
    internal class FilesystemIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly BinaryReader reader;
        private List<long> offsets;
        private int currentPosition;
        private FileCabinetRecord current;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="reader">reader.</param>
        /// <param name="offsets"> Offsets of the records in filestream. </param>
        public FilesystemIterator(BinaryReader reader, List<long> offsets)
        {
            this.reader = reader;
            this.offsets = offsets;
            this.currentPosition = -1;
            this.current = new FileCabinetRecord();
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current => this.current;

        /// <inheritdoc/>
        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (++this.currentPosition >= this.offsets.Count)
            {
                return false;
            }
            else
            {
                    this.reader.BaseStream.Seek(this.offsets[this.currentPosition], SeekOrigin.Begin);
                    this.current = FileCabinetFilesystemService.ReadRecordFromStream(reader);
            }

            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.currentPosition = -1;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.reader.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
