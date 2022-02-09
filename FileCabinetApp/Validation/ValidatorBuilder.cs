using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Utils.Config;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private const string DefaultValidationString = "default";
        private const string CustomValidationString = "custom";
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Creates validator with default rules.
        /// </summary>
        /// <returns> Record validator with default rules. </returns>
        public static IRecordValidator CreateDefault()
        {
            return new ValidationConfigReader(DefaultValidationString).ReadConfig();
        }

        /// <summary>
        /// Creates validator with custom rules.
        /// </summary>
        /// <returns> Record validator with custom rules. </returns>
        public static IRecordValidator CreateCustom()
        {
            return new ValidationConfigReader(CustomValidationString).ReadConfig();
        }

        /// <summary>
        /// Adds <see cref="FirstNameValidator"/> to validators.
        /// </summary>
        /// <param name="min"> Min length of first name. </param>
        /// <param name="max"> Max length of first name. </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds <see cref="LastNameValidator"/> to validators.
        /// </summary>
        /// <param name="min"> Min length of last name. </param>
        /// <param name="max"> Max length of last name. </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds <see cref="DateOfBirthValidator"/> to validators.
        /// </summary>
        /// <param name="from"> Min date of birth. </param>
        /// <param name="to"> Max date of birth. </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds <see cref="SexValidator"/> to validators.
        /// </summary>
        /// <param name="availableSexs"> Array of availavel sexs.  </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateSex(char[] availableSexs)
        {
            this.validators.Add(new SexValidator(availableSexs));
            return this;
        }

        /// <summary>
        /// Adds <see cref="HeightValidator"/> to validators.
        /// </summary>
        /// <param name="min"> Min height. </param>
        /// <param name="max"> Max height. </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateHeight(short min, short max)
        {
            this.validators.Add(new HeightValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds <see cref="SalaryValidator"/> to validators.
        /// </summary>
        /// <param name="min"> Min salary. </param>
        /// <param name="max"> Max salary. </param>
        /// <returns> This intance. </returns>
        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        /// <summary>
        /// Creates <see cref="CompositeValidator"/> with validators.
        /// </summary>
        /// <returns> Instance of <see cref="CompositeValidator"/>. </returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
