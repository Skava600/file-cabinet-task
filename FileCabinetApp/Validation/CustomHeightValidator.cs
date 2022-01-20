using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class CustomHeightValidator : IRecordValidator
    {
        private const short MinHeight = 60;
        private const short MaxHeight = 272;

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
