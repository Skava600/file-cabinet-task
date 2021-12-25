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
        /// <summary>This method validates  parameters from given <see cref="RecordData"/> class.</summary>
        /// <param name="record"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        public void ValidateParameters(RecordData record);
    }
}
