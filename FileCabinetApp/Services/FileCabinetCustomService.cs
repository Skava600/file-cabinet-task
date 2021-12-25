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
    /// The file cabinet service with custom behaviour.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Creates Cusom Validator.
        /// </summary>
        /// <returns>
        /// <see cref="CustomValidator"/>.
        /// </returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
