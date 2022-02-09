using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime minDate;
        private readonly DateTime maxDate;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.minDate = from;
            this.maxDate = to;
        }

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

            if (record.DateOfBirth < this.minDate || record.DateOfBirth > this.maxDate)
            {
                throw new ArgumentException($"Date of birth must be between {this.minDate.ToString("d", CultureInfo.InvariantCulture)} and {this.maxDate.ToString("d", CultureInfo.InvariantCulture)}");
            }
        }
    }
}
