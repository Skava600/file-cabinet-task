using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class HeightValidator : IRecordValidator
    {
        private readonly short minHeight;
        private readonly short maxHeight;

        public HeightValidator(short minHeight, short maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// Validates height in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.Height < this.minHeight || record.Height > this.maxHeight)
            {
                throw new ArgumentException($"Height must be a number between {this.minHeight}  and {this.maxHeight}");
            }
        }
    }
}
