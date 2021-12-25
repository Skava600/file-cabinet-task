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
        private const int MinNameLength = 3;
        private const int MaxNameLength = 80;

        private const short MinHeight = 0;
        private const short MaxHeight = 300;

        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        /// <summary>This method validates  parameters from given <see cref="RecordData"/> class.</summary>
        /// <param name="record"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the first name is null.
        /// or
        /// Thrown when the last name is null.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when first or last name less <c>MinNameLength</c> or more than <c>MaxNameLength</c>.
        /// or
        /// Thrown when first or last name consists of white spaces.
        /// or
        /// Thrown when date of birth less <c>MinDate</c> or more than Current Date.
        /// or
        /// Thrown when sex is not M or F with ignore case.
        /// or
        /// Thrown when height less <c>MinHeight</c> or more than <c>MaxHeight</c>.
        /// </exception>
        public void ValidateParameters(RecordData record)
        {
            if (record.FirstName == null)
            {
                throw new ArgumentNullException(nameof(record.FirstName));
            }

            if (record.FirstName.Length < MinNameLength ||
                record.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException(
                    $"Length of first Name must be between {MinNameLength} and {MaxNameLength}.",
                    nameof(record.FirstName));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentException(
                    "First name consists of white spaces.",
                    nameof(record.FirstName));
            }

            if (record.LastName == null)
            {
                throw new ArgumentNullException(nameof(record.LastName));
            }

            if (record.LastName.Length < MinNameLength || record.LastName.Length > MaxNameLength)
            {
                throw new ArgumentException(
                    $"Length of last Name must be between {MinNameLength} and {MaxNameLength}.",
                    nameof(record.LastName));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentException(
                    "Last name consists of white spaces.",
                    nameof(record.LastName));
            }

            if (record.DateOfBirth < MinDate || record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth current must be between {MinDate.ToShortDateString} and {DateTime.Now.ToShortDateString}.", nameof(record.DateOfBirth));
            }

            if (!char.ToUpper(record.Sex).Equals('M') && !char.ToUpper(record.Sex).Equals('F'))
            {
                throw new ArgumentException("sex is only M(male) and F(female).", nameof(record.Sex));
            }

            if (record.Height < MinHeight || record.Height > MaxHeight)
            {
                throw new ArgumentException($"height must be a number between {MinHeight}  and {MaxHeight}.", nameof(record.Height));
            }

            if (record.Salary < 0)
            {
                throw new ArgumentException("salary can't be less zero.", nameof(record.Salary));
            }
        }
    }
}
