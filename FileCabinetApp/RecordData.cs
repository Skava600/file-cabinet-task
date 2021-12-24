using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    ///  Class to introduce parameter object of FileCabinetService.CreateRecord and FileCabinetService.EditRecord.
    /// </summary>
    public class RecordData
    {
        private readonly string? firstName;
        private readonly string? lastName;
        private readonly DateTime dateOfBirth;
        private readonly char sex;
        private readonly short height;
        private readonly decimal salary;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordData"/> class.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="sex">The sex.</param>
        /// <param name="height">The height.</param>
        /// <param name="salary">The salary.</param>
        public RecordData(string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.dateOfBirth = dateOfBirth;
            this.sex = sex;
            this.height = height;
            this.salary = salary;
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string? FirstName { get => this.firstName; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string? LastName { get => this.lastName; }

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get => this.dateOfBirth; }

        /// <summary>
        /// Gets the sex.
        /// </summary>
        /// <value>
        /// The sex.
        /// </value>
        public char Sex { get => this.sex; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public short Height { get => this.height; }

        /// <summary>
        /// Gets the salary.
        /// </summary>
        /// <value>
        /// The salary.
        /// </value>
        public decimal Salary { get => this.salary; }
    }
}
