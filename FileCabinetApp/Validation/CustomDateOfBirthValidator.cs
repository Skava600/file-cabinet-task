using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class CustomDateOfBirthValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private static readonly DateTime MaxDate = DateTime.Now;

        /// <summary>
        /// Validates date of birth in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.DateOfBirth < MinDate || record.DateOfBirth > MaxDate)
            {
                throw new ArgumentException($"Date of birth current must be between {MinDate.ToShortDateString} and {MaxDate.ToShortDateString}");
            }
        }
    }
}
