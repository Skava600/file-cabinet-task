using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Utils.Config;

namespace FileCabinetApp.Validation
{
    public class ValidatorBuilder
    {
        private const string DefaultValidationString = "default";
        private const string CustomValidationString = "custom";
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        public IRecordValidator CreateDefault()
        {
            return new ValidationConfigReader(DefaultValidationString).ReadConfig();
        }

        public IRecordValidator CreateCustom()
        {
            return new ValidationConfigReader(CustomValidationString).ReadConfig();
        }

        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        public ValidatorBuilder ValidateSex(char[] availableSexs)
        {
            this.validators.Add(new SexValidator(availableSexs));
            return this;
        }

        public ValidatorBuilder ValidateHeight(short min, short max)
        {
            this.validators.Add(new HeightValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
