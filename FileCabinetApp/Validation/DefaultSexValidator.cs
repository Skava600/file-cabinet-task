using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class DefaultSexValidator : IRecordValidator
    {
        private static readonly char[] AvailableSex = new char[] { 'M', 'F', 'N' };

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

            if (!Array.Exists(AvailableSex, sex => sex.Equals(char.ToUpperInvariant(sex))))
            {
                throw new ArgumentException("sex is only M(male) and F(female) and None(N)");
            }
        }
    }
}
