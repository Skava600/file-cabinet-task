using System.Globalization;
using System.Xml;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.Writers
{
    /// <summary>
    /// Class that exports file cabinet record to XML.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer"> The text writer. </param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes a record to a xml file.
        /// </summary>
        /// <param name="record"> file cabinet record. </param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteStartElement("record");

            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.CurrentCulture));

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();

            this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            this.writer.WriteElementString("sex", record.Sex.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteElementString("height", record.Height.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteElementString("salary", record.Salary.ToString(CultureInfo.InvariantCulture));

            this.writer.WriteEndElement();
        }
    }
}
