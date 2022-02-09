using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// First name validator.
    /// </summary>
    internal class FirstNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength"> Min length of first name. </param>
        /// <param name="maxLength"> Max length of first name. </param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minNameLength = minLength;
            this.maxNameLength = maxLength;
        }

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

            if (record.FirstName.Length < this.minNameLength ||
               record.FirstName.Length > this.maxNameLength)
            {
                throw new ArgumentException($"Length of first name must be between {this.minNameLength} and {this.maxNameLength}");
            }
        }
    }
}
