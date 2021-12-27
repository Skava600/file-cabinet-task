using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface to describe the file cabinet service.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// This method creates new FileCabinetRecord with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        /// <returns>return a number representing id of the new record.</returns>
        int CreateRecord(RecordData recordData);

        /// <summary>
        /// This method edites FileCabinetRecord found by id with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="id">ID of editing record.</param>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        void EditRecord(int id, RecordData recordData);

        /// <summary>This method for getting all records.</summary>
        /// <returns>Read only collection of registered <see cref="FileCabinetRecord"/>.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>This method checks if the record with given id exists.</summary>
        /// <param name="id">id of record.</param>
        /// <returns><c>true</c> if record exists and <c>false</c> otherwise.</returns>
        bool IsRecordExists(int id);

        /// <summary>This method for getting quantity of registered records.</summary>
        /// <returns>int number of records.</returns>
        int GetStat();

        /// <summary>
        /// Finds the specified records with property name and value.
        /// </summary>
        /// <param name="property">Name and value of property through white space.</param>
        /// <returns>All records with specified last name.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByProperty(string property);
    }
}
