using System.Xml;
using FileCabinetApp.Entities;
using FileCabinetApp.Utils.Writers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class for representing file cabinet service snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records"> array of records. </param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Saves a records to a csv file.
        /// </summary>
        /// <param name="streamWriter"> Stream for exporting. </param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            streamWriter.WriteLine("Id, First name, Last name, Date of birth, Sex, Height, Salary");

            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Saves a records to a xml file.
        /// </summary>
        /// <param name="streamWriter"> Stream for exporting. </param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, settings);

            var fileCabinetStreamWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            foreach (var record in this.records)
            {
                fileCabinetStreamWriter.Write(record);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }
    }
}
