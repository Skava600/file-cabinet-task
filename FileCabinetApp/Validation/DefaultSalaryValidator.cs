using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class DefaultSalaryValidator : IRecordValidator
    {
        private const decimal MinSalary = 500;
        private const decimal MaxSalary = 50000;

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
