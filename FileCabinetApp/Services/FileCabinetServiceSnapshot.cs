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
    }
}
