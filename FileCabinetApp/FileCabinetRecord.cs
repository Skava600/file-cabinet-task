using System;

namespace FileCabinetApp
{
    /// <summary>
    /// The class to subscribe file cabinet record.
    /// </summary>
    public class FileCabinetRecord
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
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string? LastName { get; set; }

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
    }
}
