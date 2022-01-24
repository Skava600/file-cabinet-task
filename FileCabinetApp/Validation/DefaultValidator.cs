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
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator()
           : base(new IRecordValidator[]
       {
            new FirstNameValidator(3, 60),
            new LastNameValidator(3, 60),
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now),
            new SexValidator(new char[] { 'M', 'F' }),
            new HeightValidator(0, 300),
            new SalaryValidator(500, 50000),
       })
        {
        }
    }
}