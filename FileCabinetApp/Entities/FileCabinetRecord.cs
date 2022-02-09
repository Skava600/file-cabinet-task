using System;

namespace FileCabinetApp.Entities
{
    /// <summary>
    /// The class to subscribe file cabinet record.
    /// </summary>
    public class FileCabinetRecord : IEquatable<FileCabinetRecord>
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the sex (M or F).
        /// </summary>
        /// <value>
        /// The sex M or F.
        /// </value>
        public char Sex { get; set; }

        /// <summary>
        /// Gets or sets the height in cm.
        /// </summary>
        /// <value>
        /// The height in cm.
        /// </value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets the salary in dollars.
        /// </summary>
        /// <value>
        /// The salary in dollars.
        /// </value>
        public decimal Salary { get; set; }

        /// <inheritdoc/>
        public bool Equals(FileCabinetRecord? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id) &&
                this.FirstName.Equals(other.FirstName, StringComparison.OrdinalIgnoreCase) &&
                this.LastName.Equals(other.LastName, StringComparison.OrdinalIgnoreCase) &&
                this.DateOfBirth.Equals(other.DateOfBirth) &&
                char.ToUpperInvariant(this.Sex).Equals(char.ToUpperInvariant(other.Sex)) &&
                this.Height.Equals(other.Height) &&
                this.Salary.Equals(other.Salary);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            FileCabinetRecord? record = obj as FileCabinetRecord;
            if (record == null)
            {
                return false;
            }
            else
            {
                return this.Equals(record);
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                (this.FirstName == null ? 0 : this.FirstName.GetHashCode(StringComparison.OrdinalIgnoreCase)) ^
                (this.LastName == null ? 0 : this.LastName.GetHashCode(StringComparison.OrdinalIgnoreCase)) ^
                this.Sex.GetHashCode() ^
                this.DateOfBirth.GetHashCode() ^
                this.Height.GetHashCode() ^
                this.Salary.GetHashCode();
        }
    }
}
