using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class CustomFirstNameValidator : IRecordValidator
    {
        private const int MinNameLength = 3;
        private const int MaxNameLength = 50;

        /// <summary>
        /// Validates first name in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.FirstName is null)
            {
                throw new ArgumentException("First name can't be null");
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentException("First name consists of white spaces.");
            }

            if (record.FirstName.Length < MinNameLength ||
               record.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Length of first name must be between {MinNameLength} and {MaxNameLength}");
            }
        }
    }
}
