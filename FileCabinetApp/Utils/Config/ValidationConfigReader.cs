using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Validation;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Utils.Config
{
    internal class ValidationConfigReader
    {
        private const string ValidationConfigFile = "validation-rules.json";
        private string validationRules;
        private IConfiguration config;

        public ValidationConfigReader(string validationRules)
        {
            this.validationRules = validationRules;
            this.config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ValidationConfigFile, true, true)
            .Build();
        }

        public IRecordValidator ReadConfig()
        {
            var firstNameRules = this.ReadFirstNameRules();
            var lastNameRules = this.ReadLastNameRules();
            var dateOfBirthRules = this.ReadDateOfBirthRules();
            var heightRules = this.ReadHeightRules();
            var sexRules = this.ReadSexRules();
            var salaryRules = this.ReadSalaryRules();

            return new ValidatorBuilder()
                .ValidateFirstName(firstNameRules.Item1, firstNameRules.Item2)
                .ValidateLastName(lastNameRules.Item1, lastNameRules.Item2)
                .ValidateDateOfBirth(dateOfBirthRules.Item1, dateOfBirthRules.Item2)
                .ValidateSex(sexRules)
                .ValidateHeight(heightRules.Item1, heightRules.Item2)
                .ValidateSalary(salaryRules.Item1, salaryRules.Item2)
                .Create();
        }

        private Tuple<int, int> ReadFirstNameRules()
        {
            var minSize = this.config.GetSection(this.validationRules).GetSection("firstName:min");
            var maxSize = this.config.GetSection(this.validationRules).GetSection("firstName:max");
            return new Tuple<int, int>(minSize.Get<int>(), maxSize.Get<int>());
        }

        private Tuple<int, int> ReadLastNameRules()
        {
            var minSize = this.config.GetSection(this.validationRules).GetSection("lastName:min");
            var maxSize = this.config.GetSection(this.validationRules).GetSection("lastName:max");
            return new Tuple<int, int>(minSize.Get<int>(), maxSize.Get<int>());
        }

        private Tuple<DateTime, DateTime> ReadDateOfBirthRules()
        {
            var from = this.config.GetSection(this.validationRules).GetSection("dateOfBirth:from");
            var to = this.config.GetSection(this.validationRules).GetSection("dateOfBirth:to");
            return new Tuple<DateTime, DateTime>(from.Get<DateTime>(), to.Get<DateTime>());
        }

        private Tuple<short, short> ReadHeightRules()
        {
            var minHeight = this.config.GetSection(this.validationRules).GetSection("height:min");
            var maxHeight = this.config.GetSection(this.validationRules).GetSection("height:max");
            return new Tuple<short, short>(minHeight.Get<short>(), maxHeight.Get<short>());
        }

        private Tuple<decimal, decimal> ReadSalaryRules()
        {
            var minSalary = this.config.GetSection(this.validationRules).GetSection("salary:min");
            var maxSalary = this.config.GetSection(this.validationRules).GetSection("salary:max");
            return new Tuple<decimal, decimal>(minSalary.Get<decimal>(), maxSalary.Get<decimal>());
        }

        private char[] ReadSexRules()
        {
            var sexRules = this.config.GetSection(this.validationRules).GetSection("sex");
            return sexRules.Get<char[]>();
        }
    }
}
