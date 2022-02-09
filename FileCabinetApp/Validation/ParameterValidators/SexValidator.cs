using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Sex validator.
    /// </summary>
    internal class SexValidator : IRecordValidator
    {
        private readonly char[] availableSex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SexValidator"/> class.
        /// </summary>
        /// <param name="availableSex"> array of available sexs. </param>
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

            if (!Array.Exists(this.availableSex, sex => char.ToUpperInvariant(sex).Equals(char.ToUpperInvariant(record.Sex))))
            {
                throw new ArgumentException("Wrong sex");
            }
        }
    }
}
