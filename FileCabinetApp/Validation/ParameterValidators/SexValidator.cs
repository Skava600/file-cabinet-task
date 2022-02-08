using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class SexValidator : IRecordValidator
    {
        private readonly char[] availableSex;

        public SexValidator(char[] availableSex)
        {
            this.availableSex = availableSex;
        }

        /// <summary>
        /// Validates sex in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!Array.Exists(this.availableSex, sex => sex.Equals(char.ToUpperInvariant(record.Sex))))
            {
                throw new ArgumentException("Wrong sex");
            }
        }
    }
}
