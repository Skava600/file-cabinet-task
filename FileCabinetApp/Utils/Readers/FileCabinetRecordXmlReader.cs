using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

namespace FileCabinetApp.Utils.Readers
{
    /// <summary>
    /// Class for reading records from xml reader in xml format.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader"> Stream reader. </param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Read all records from stream reader.
        /// </summary>
        /// <returns> Collection of FileCabinetRecord. </returns>
        public IList<FileCabinetRecord> ReadAlL()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();
            XmlSerializer serializer = new XmlSerializer(typeof(RecordsSerializable));

            RecordsSerializable recordsSerializable = (RecordsSerializable)serializer.Deserialize(this.reader) !;

            foreach (var recordSerializable in recordsSerializable.Records)
            {
                records.Add(new FileCabinetRecord
                {
                    Id = recordSerializable.Id,
                    FirstName = recordSerializable.Name != null ? recordSerializable.Name.FirstName : string.Empty,
                    LastName = recordSerializable.Name != null ? recordSerializable.Name.LastName : string.Empty,
                    DateOfBirth = recordSerializable.DateOfBirth,
                    Sex = recordSerializable.Sex,
                    Height = recordSerializable.Height,
                    Salary = recordSerializable.Salary,
                });
            }

            return records;
        }
    }
}
