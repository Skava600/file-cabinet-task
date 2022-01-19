using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.RecordPrinters
{
    /// <summary>
    /// Default record printer.
    /// </summary>
    internal class DefaultRecordPrinter : IRecordPrinter
    {
        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, " +
                    $"{record.FirstName}, " +
                    $"{record.LastName}, " +
                    $"{date}, {record.Sex}, " +
                    $"{record.Height}, " +
                    $"{record.Salary}");
            }
        }
    }
}
