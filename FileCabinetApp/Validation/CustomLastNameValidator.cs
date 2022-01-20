using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class CustomLastNameValidator : IRecordValidator
    {
        private const int MinNameLength = 3;
        private const int MaxNameLength = 50;

        /// <summary>
        /// Validates last name in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.LastName is null)
            {
                throw new ArgumentException("Last name can't be null");
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentException("Last name consists of white spaces.");
            }

            if (record.LastName.Length < MinNameLength ||
               record.LastName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Length of last name must be between {MinNameLength} and {MaxNameLength}");
            }
        }
    }
}
