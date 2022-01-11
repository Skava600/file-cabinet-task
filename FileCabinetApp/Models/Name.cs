using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp.Models
{
    /// <summary>
    /// Class for full name representation.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Gets or sets first name of a record.
        /// </summary>
        /// <value>
        /// first name of a record.
        /// </value>
        [XmlAttribute("first")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of a record.
        /// </summary>
        /// <value>
        /// last name of a record.
        /// </value>
        [XmlAttribute("last")]
        public string? LastName { get; set; }
    }
}
