using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.RecordPrinters
{
    /// <summary>
    /// Record printer interface.
    /// </summary>
    internal interface IRecordPrinter
    {
        /// <summary>
        /// Prints file cabinet records.
        /// </summary>
        /// <param name="records"> file cabinet record. </param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
