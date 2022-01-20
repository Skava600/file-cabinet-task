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
            new CustomFirstNameValidator().ValidateParameters(record);
            new CustomLastNameValidator().ValidateParameters(record);
            new CustomDateOfBirthValidator().ValidateParameters(record);
            new CustomSexValidator().ValidateParameters(record);
            new CustomHeightValidator().ValidateParameters(record);
            new CustomSalaryValidator().ValidateParameters(record);
        }
    }
}