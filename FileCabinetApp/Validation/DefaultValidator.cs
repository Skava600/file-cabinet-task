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
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinHeight = 60;
        private const short MaxHeight = 272;

        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);

        /// <inheritdoc/>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (firstName is null)
            {
                return new Tuple<bool, string>(false, "First name can't be null");
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                return new Tuple<bool, string>(false, "First name consists of white spaces.");
            }

            if (firstName.Length < MinNameLength ||
               firstName.Length > MaxNameLength)
            {
                return new Tuple<bool, string>(false, $"Length of first name must be between {MinNameLength} and {MaxNameLength}");
            }

            return new Tuple<bool, string>(true, nameof(firstName));
        }

        /// <inheritdoc/>
        public Tuple<bool, string> LastNameValidator(string lastName)
        {
            if (lastName is null)
            {
                return new Tuple<bool, string>(false, "Last name can't be null");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return new Tuple<bool, string>(false, "Last name consists of white spaces.");
            }

            if (lastName.Length < MinNameLength ||
               lastName.Length > MaxNameLength)
            {
                return new Tuple<bool, string>(false, $"Length of last name must be between {MinNameLength} and {MaxNameLength}");
            }

            return new Tuple<bool, string>(true, nameof(lastName));
        }

        /// <inheritdoc/>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth < MinDate || dateOfBirth > DateTime.Now)
            {
                return new Tuple<bool, string>(
                    false,
                    $"Date of birth current must be between {MinDate.ToShortDateString} and {DateTime.Now.ToShortDateString}");
            }

            return new Tuple<bool, string>(true, nameof(dateOfBirth));
        }

        /// <inheritdoc/>
        public Tuple<bool, string> SexValidator(char sex)
        {
            if (!char.ToUpper(sex).Equals('M') && !char.ToUpper(sex).Equals('F'))
            {
                return new Tuple<bool, string>(false, "sex is only M(male) and F(female)");
            }

            return new Tuple<bool, string>(true, nameof(sex));
        }

        /// <inheritdoc/>
        public Tuple<bool, string> HeightValidator(short height)
        {
            if (height < MinHeight || height > MaxHeight)
            {
                return new Tuple<bool, string>(false, $"height must be a number between {MinHeight}  and {MaxHeight}");
            }

            return new Tuple<bool, string>(true, nameof(height));
        }

        /// <inheritdoc/>
        public Tuple<bool, string> SalaryValidator(decimal salary)
        {
            if (salary < 0)
            {
                return new Tuple<bool, string>(false, "salary can't be less zero");
            }

            return new Tuple<bool, string>(true, nameof(salary));
        }
    }
}