using System;
using System.Collections.Generic;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class to describe the file cabinet service.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary =
            new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// This method creates new FileCabinetRecord with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        /// <returns>return a number representing id of the new record.</returns>
        public int CreateRecord(RecordData recordData)
        {
            this.CreateValidator().ValidateParameters(recordData);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Sex = char.ToUpper(recordData.Sex),
                Height = recordData.Height,
                Salary = recordData.Salary,
            };

            this.list.Add(record);

            this.AddRecordToDictionaries(record.FirstName !, record.LastName !, record.DateOfBirth, record);

            return record.Id;
        }

        /// <summary>
        /// This method creates validator.
        /// </summary>
        /// <returns>
        /// <see cref="IRecordValidator"/>.
        /// </returns>
        public abstract IRecordValidator CreateValidator();

        /// <summary>
        /// This method edites FileCabinetRecord found by id with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="id">ID of editing record.</param>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        public void EditRecord(int id, RecordData recordData)
        {
            FileCabinetRecord record = this.list.Find(rec => rec.Id == id)
                ?? throw new ArgumentOutOfRangeException(nameof(id), $"#{id} record is not found");

            this.CreateValidator().ValidateParameters(recordData);

            this.firstNameDictionary[record.FirstName !].Remove(record);
            this.lastNameDictionary[record.LastName!].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

            record.FirstName = recordData.FirstName;
            record.LastName = recordData.LastName;
            record.DateOfBirth = recordData.DateOfBirth;
            record.Sex = recordData.Sex;
            record.Height = recordData.Height;
            record.Salary = recordData.Salary;

            this.AddRecordToDictionaries(recordData.FirstName!, recordData.LastName!, recordData.DateOfBirth, record);
        }

        /// <summary>This method for getting array of records.</summary>
        /// <returns>array of registered FileCabinetRecord.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>This method checks if the record with given id exists.</summary>
        /// <param name="id">id of record.</param>
        /// <returns><c>true</c> if record exists and <c>false</c> otherwise.</returns>
        public bool IsRecordExists(int id)
        {
            return this.list.Exists(record => record.Id == id);
        }

        /// <summary>This method for getting quantity of registered records.</summary>
        /// <returns>int number of reg.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Finds the specified first name.
        /// </summary>
        /// <param name="firstName">The first name of record.</param>
        /// <returns>All records with specified first name.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the specified last name.
        /// </summary>
        /// <param name="lastName">The last name of record.</param>
        /// <returns>All records with specified last name.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth of record.</param>
        /// <returns>All records with specified date of birth.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime.TryParse(dateOfBirth, out DateTime dob);
            if (this.dateOfBirthDictionary.ContainsKey(dob))
            {
                return this.dateOfBirthDictionary[dob].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        private void AddRecordToDictionaries(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord record)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstName].Add(record);
            this.lastNameDictionary[lastName].Add(record);
            this.dateOfBirthDictionary[dateOfBirth].Add(record);
        }
    }
}
