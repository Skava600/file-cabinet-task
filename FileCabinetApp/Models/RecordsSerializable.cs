using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp.Models
{
    /// <summary>
    /// Model for record serialization".
    /// </summary>
    [XmlRoot("records")]
    public class RecordsSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsSerializable"/> class.
        /// </summary>
        /// <param name="records">
        /// File Cabinet records to serialize.
        /// </param>
        public RecordsSerializable(List<RecordSerializable> records)
        {
            this.Records = records;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsSerializable"/> class.
        /// </summary>
        public RecordsSerializable()
        {
            this.Records = new List<RecordSerializable>();
        }

        /// <summary>
        /// Gets records to serialize.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        [XmlElement("record")]
        public List<RecordSerializable> Records { get; }
    }
}
