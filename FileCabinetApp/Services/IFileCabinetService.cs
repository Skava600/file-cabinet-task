using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Services;
using FileCabinetApp.Utils.Iterators;

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

        /// <summary>
        /// This method removes FileCabinetRecord from service with given id.
        /// </summary>
        /// <param name="id"> Id of removing record. </param>
        void RemoveRecord(int id);

        /// <summary>
        /// This method defragmentate data file from deleted records.
        /// </summary>
        void Purge();

        /// <summary>
        /// This method for getting all records.
        /// </summary>
        /// <returns>Read only collection of registered <see cref="FileCabinetRecord"/>.</returns>
        IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// This method checks if the record with given id exists.
        /// </summary>
        /// <param name="id">id of record.</param>
        /// <returns> <c>true</c> if record exists and <c>false</c> otherwise.</returns>
        bool IsRecordExists(int id);

        /// <summary>
        /// This method for getting quantity of registered records.
        /// </summary>
        /// <returns> Tuple, where first compomet is - number of records and second - number of deleted records. </returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// This method creates a snapshot of a file cabinet service.
        /// </summary>
        /// <returns> <see cref="FileCabinetServiceSnapshot"/>.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Finds the specified records by firstname.
        /// </summary>
        /// <param name="firstname"> First Name of record. </param>
        /// <returns> All records with specified first name. </returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstname);

        /// <summary>
        /// Finds the specified records by lastname.
        /// </summary>
        /// <param name="lastname"> Last Name of record. </param>
        /// <returns> All records with specified last name. </returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastname);

        /// <summary>
        /// Finds the specified records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth"> Date of birth of record. </param>
        /// <returns> All records with specified date of birth. </returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Restores records from snapshot.
        /// </summary>
        /// <param name="snapshot"> <see cref="FileCabinetServiceSnapshot"/>. </param>
        void Restore(FileCabinetServiceSnapshot snapshot);
    }
}
