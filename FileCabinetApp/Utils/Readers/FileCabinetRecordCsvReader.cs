using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.Readers
{
    /// <summary>
    /// Class for reading records from stream reader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader"> Stream reader. </param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Read all records from stream reader.
        /// </summary>
        /// <returns> Collection of FileCabinetRecord. </returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();
            string? line;
            while ((line = this.reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                FileCabinetRecord record = new FileCabinetRecord
                {
                    Id = int.Parse(fields[0]),
                    FirstName = fields[1],
                    LastName = fields[2],
                    DateOfBirth = DateTime.Parse(fields[3], CultureInfo.InvariantCulture),
                    Sex = char.Parse(fields[4].Trim()),
                    Height = short.Parse(fields[5]),
                    Salary = decimal.Parse(fields[6]),
                };
                records.Add(record);
            }

            return records;
        }
    }
}
