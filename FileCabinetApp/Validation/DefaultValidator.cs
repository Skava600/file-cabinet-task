using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class for dafault validation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            new FirstNameValidator(3, 60).ValidateParameters(record);
            new LastNameValidator(3, 60).ValidateParameters(record);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParameters(record);
            new SexValidator(new char[] { 'M', 'F' }).ValidateParameters(record);
            new HeightValidator(0, 300).ValidateParameters(record);
            new SalaryValidator(500, 50000).ValidateParameters(record);
        }
    }
}