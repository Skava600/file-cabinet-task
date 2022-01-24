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
    public class CustomValidator : CompositeValidator
    {
        public CustomValidator()
            : base(new IRecordValidator[]
        {
            new FirstNameValidator(2, 50),
            new LastNameValidator(2, 50),
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now),
            new SexValidator(new char[] { 'M', 'F', 'N' }),
            new HeightValidator(60, 272),
            new SalaryValidator(0, decimal.MaxValue),
        })
        {
        }
    }
}