using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class for custom validation.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(RecordData record)
        {
            new FirstNameValidator(2, 50).ValidateParameters(record);
            new LastNameValidator(2, 50).ValidateParameters(record);
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now).ValidateParameters(record);
            new SexValidator(new char[] { 'M', 'F', 'N' }).ValidateParameters(record);
            new HeightValidator(60, 272).ValidateParameters(record);
            new SalaryValidator(0, decimal.MaxValue).ValidateParameters(record);
        }
    }
}