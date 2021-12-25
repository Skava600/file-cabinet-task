using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The file cabinet service with default behaviour.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Creates Default Validator.
        /// </summary>
        /// <returns>
        /// <see cref="DefaultValidator"/>.
        /// </returns>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
