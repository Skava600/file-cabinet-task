using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Models
{
    /// <summary>
    /// Class For serialization of <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class RecordSerializable
    {
        /// <summary>
        /// Gets or sets the name of record.
        /// </summary>
        /// <value>
        /// The name of record.
        /// </value>
        [XmlElement("name", IsNullable = false)]
        public Name? Name { get; set; }

        /// <summary>
        /// Gets or sets the id of record.
        /// </summary>
        /// <value>
        /// The id of record.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of record.
        /// </summary>
        /// <value>
        /// The date of birth of record.
        /// </value>
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the sex of record.
        /// </summary>
        /// <value>
        /// The sex of record.
        /// </value>
        [XmlElement("sex")]
        public char Sex { get; set; }

        /// <summary>
        /// Gets or sets the height of a record.
        /// </summary>
        /// <value>
        /// The height of a record.
        /// </value>
        [XmlElement("height")]
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets the salary of record.
        /// </summary>
        /// <value>
        /// The salary of a record.
        /// </value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Overriding method ToString representation of serialization model.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            return $"{this.Id}, " +
                $"{this.Name?.FirstName}, " +
                $"{this.Name?.LastName}, " +
                $"{this.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}, " +
                $"{this.Sex}, " +
                $"{this.Height}, " +
                $"{this.Salary}";
        }
    }
}
