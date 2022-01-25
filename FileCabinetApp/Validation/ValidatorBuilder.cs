using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        public IRecordValidator CreateDefault()
        {
            return new ValidatorBuilder()
                .ValidateFirstName(3, 60)
                .ValidateLastName(3, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateSex(new char[] { 'M', 'F' })
                .ValidateHeight(0, 300)
                .ValidateSalary(500, 50000)
                .Create();
        }

        public IRecordValidator CreateCustom()
        {
            return new ValidatorBuilder()
               .ValidateFirstName(2, 50)
               .ValidateLastName(2, 50)
               .ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Now)
               .ValidateSex(new char[] { 'M', 'F', 'N' })
               .ValidateHeight(50, 272)
               .ValidateSalary(0, decimal.MaxValue)
               .Create();
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
