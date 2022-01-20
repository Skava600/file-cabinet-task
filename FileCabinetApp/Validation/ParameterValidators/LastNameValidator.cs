using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class LastNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;

        public LastNameValidator(int minLength, int maxLength)
        {
            this.minNameLength = minLength;
            this.maxNameLength = maxLength;
        }

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

            if (record.LastName.Length < this.minNameLength ||
               record.LastName.Length > this.maxNameLength)
            {
                throw new ArgumentException($"Length of last name must be between {this.minNameLength} and {this.maxNameLength}");
            }
        }
    }
}
