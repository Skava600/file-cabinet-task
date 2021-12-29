using FileCabinetApp.Entities;
using System.Globalization;

namespace FileCabinetApp.Utils.Writers
{
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer"> The Text. </param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes a record to a csv file.
        /// </summary>
        /// <param name="record"> file cabinet record. </param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteLine(
                $"{record.Id}," +
                $"{record.FirstName}," +
                $"{record.LastName}," +
                $"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}," +
                $"{record.Sex}," +
                $"{record.Height} cm," +
                $"{record.Salary} $");
        }
    }
}
