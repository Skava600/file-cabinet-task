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
        /// This method validates the first name.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <returns>Validation result. </returns>
        Tuple<bool, string> FirstNameValidator(string firstName);

        /// <summary>
        /// This method validates the last name.
        /// </summary>
        /// <param name="lastName"> The last name. </param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> LastNameValidator(string lastName);

        /// <summary>
        /// This method validates the date of birth.
        /// </summary>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth);

        /// <summary>
        /// This method validates sex.
        /// </summary>
        /// <param name="sex"> The sex. </param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> SexValidator(char sex);

        /// <summary>
        /// This method validates the height.
        /// </summary>
        /// <param name="height"> The height. </param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> HeightValidator(short height);

        /// <summary>
        /// Whis method validates salary.
        /// </summary>
        /// <param name="salary"> The salary. </param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> SalaryValidator(decimal salary);
    }
}
