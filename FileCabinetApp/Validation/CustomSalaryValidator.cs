using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class CustomSalaryValidator : IRecordValidator
    {
        private const decimal MinSalary = 0;
        private const decimal MaxSalary = decimal.MaxValue;

        /// <summary>
        /// Validates salary in parameters data.
        /// </summary>
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.Salary < MinSalary || record.Salary > decimal.MaxValue)
            {
                throw new ArgumentException($"Salary should be between {MinSalary} and {MaxSalary}.");
            }
        }
    }
}
