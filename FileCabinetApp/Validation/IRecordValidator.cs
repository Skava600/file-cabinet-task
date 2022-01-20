using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// This interface can validate parameters from RecordData.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// This method validates fields of <see cref="RecordData"/>.
        /// </summary>
        /// <param name="record">Record data.</param>
        void ValidateParameters(RecordData record);
    }
}
