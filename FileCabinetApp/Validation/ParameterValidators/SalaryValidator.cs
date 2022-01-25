using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    internal class SalaryValidator : IRecordValidator
    {
        private readonly decimal minSalary;
        private readonly decimal maxSalary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minSalary"> Min. salary. </param>
        /// <param name="maxSalary"> Max salary. </param>
        public SalaryValidator(decimal minSalary, decimal maxSalary)
        {
            this.minSalary = minSalary;
            this.maxSalary = maxSalary;
        }

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

            if (record.Salary < this.minSalary || record.Salary > decimal.MaxValue)
            {
                throw new ArgumentException($"Salary should be between {this.minSalary} and {this.maxSalary}.");
            }
        }
    }
}
