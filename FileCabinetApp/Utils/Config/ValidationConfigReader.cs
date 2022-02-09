using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Validation;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Utils.Config
{
    /// <summary>
    /// Config reader.
    /// </summary>
    internal class ValidationConfigReader
    {
        private const string ValidationConfigFile = "validation-rules.json";
        private string validationRules;
        private IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationConfigReader"/> class.
        /// </summary>
        /// <param name="validationRules"> Validaton rules. </param>
        public ValidationConfigReader(string validationRules)
        {
            this.validationRules = validationRules;
            this.config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ValidationConfigFile, true, true)
            .Build();
        }

        /// <summary>
        /// Reads validaton rules from config file.
        /// </summary>
        /// <returns> Record validator. </returns>
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

        /// <summary>
        /// Read first name rules.
        /// </summary>
        /// <returns> (Min length, Max length). </returns>
        public Tuple<int, int> ReadFirstNameRules()
        {
            var minSize = this.config.GetSection(this.validationRules).GetSection("firstName:min");
            var maxSize = this.config.GetSection(this.validationRules).GetSection("firstName:max");
            return new Tuple<int, int>(minSize.Get<int>(), maxSize.Get<int>());
        }

        /// <summary>
        /// Read last name rules.
        /// </summary>
        /// <returns> (Min length, Max length). </returns>
        public Tuple<int, int> ReadLastNameRules()
        {
            var minSize = this.config.GetSection(this.validationRules).GetSection("lastName:min");
            var maxSize = this.config.GetSection(this.validationRules).GetSection("lastName:max");
            return new Tuple<int, int>(minSize.Get<int>(), maxSize.Get<int>());
        }

        /// <summary>
        /// Read date of birth rules.
        /// </summary>
        /// <returns> (Min date, Max date). </returns>
        public Tuple<DateTime, DateTime> ReadDateOfBirthRules()
        {
            var from = this.config.GetSection(this.validationRules).GetSection("dateOfBirth:from");
            var to = this.config.GetSection(this.validationRules).GetSection("dateOfBirth:to");
            return new Tuple<DateTime, DateTime>(from.Get<DateTime>(), to.Get<DateTime>());
        }

        /// <summary>
        /// Read height rules.
        /// </summary>
        /// <returns> (Min height, Max height). </returns>
        public Tuple<short, short> ReadHeightRules()
        {
            var minHeight = this.config.GetSection(this.validationRules).GetSection("height:min");
            var maxHeight = this.config.GetSection(this.validationRules).GetSection("height:max");
            return new Tuple<short, short>(minHeight.Get<short>(), maxHeight.Get<short>());
        }

        /// <summary>
        /// Read salary rules.
        /// </summary>
        /// <returns> (Min salary, Max salary). </returns>
        public Tuple<decimal, decimal> ReadSalaryRules()
        {
            var minSalary = this.config.GetSection(this.validationRules).GetSection("salary:min");
            var maxSalary = this.config.GetSection(this.validationRules).GetSection("salary:max");
            return new Tuple<decimal, decimal>(minSalary.Get<decimal>(), maxSalary.Get<decimal>());
        }

        /// <summary>
        /// Read sex rules.
        /// </summary>
        /// <returns> array of availave sexs. </returns>
        public char[] ReadSexRules()
        {
            var sexRules = this.config.GetSection(this.validationRules).GetSection("sex");
            return sexRules.Get<char[]>();
        }
    }
}
