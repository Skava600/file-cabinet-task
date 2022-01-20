using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class DefaultHeightValidator : IRecordValidator
    {
        private const short MinHeight = 0;
        private const short MaxHeight = 300;

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

            if (record.Height < MinHeight || record.Height > MaxHeight)
            {
                throw new ArgumentException($"height must be a number between {MinHeight}  and {MaxHeight}");
            }
        }
    }
}
