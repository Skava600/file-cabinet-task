using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The file cabinet service with default behaviour.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinHeight = 60;
        private const short MaxHeight = 272;

        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        /// <summary>This method validates  parameters from given <see cref="RecordData"/> class.</summary>
        /// <param name="record"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        public override void ValidateRecordParams(RecordData record)
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
                throw new ArgumentException($"Date of birth must be between {MinDate} and {DateTime.Now}.", nameof(record.DateOfBirth));
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
