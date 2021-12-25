using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class to describe the file cabinet service.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary =
            new Dictionary<DateTime, List<FileCabinetRecord>>();

        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="recordValidator"><see cref="IRecordValidator"/>.</param>
        public FileCabinetService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// This method creates new FileCabinetRecord with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        /// <returns>return a number representing id of the new record.</returns>
        public int CreateRecord(RecordData recordData)
        {
            this.recordValidator.ValidateParameters(recordData);
            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Sex = char.ToUpper(recordData.Sex),
                Height = recordData.Height,
                Salary = recordData.Salary,
            };

            this.records.Add(record);

            this.AddRecordToDictionaries(record.FirstName !, record.LastName !, record.DateOfBirth, record);

            return record.Id;
        }

        /// <summary>
        /// This method edites FileCabinetRecord found by id with given <see cref="RecordData"/> class params.
        /// </summary>
        /// <param name="id">ID of editing record.</param>
        /// <param name="recordData"><see cref="RecordData"/> with params for FileCabinetRecord.</param>
        public void EditRecord(int id, RecordData recordData)
        {
            FileCabinetRecord record = this.records.Find(rec => rec.Id == id)
                ?? throw new ArgumentOutOfRangeException(nameof(id), $"#{id} record is not found");

            this.recordValidator.ValidateParameters(recordData);

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

        /// <summary>This method for getting all records.</summary>
        /// <returns>Read onlu collection of registered <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.records);
        }

        /// <summary>This method checks if the record with given id exists.</summary>
        /// <param name="id">id of record.</param>
        /// <returns><c>true</c> if record exists and <c>false</c> otherwise.</returns>
        public bool IsRecordExists(int id)
        {
            return this.records.Exists(record => record.Id == id);
        }

        /// <summary>This method for getting quantity of registered records.</summary>
        /// <returns>int number of records.</returns>
        public int GetStat()
        {
            return this.records.Count;
        }

        /// <summary>
        /// Finds the specified records with property name and value.
        /// </summary>
        /// <param name="property">Name and value of property through white space.</param>
        /// <returns>All records with specified last name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByProperty(string property)
        {
            string[] inputs = property.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            int nameIndex = 0;
            string propertyName = inputs[nameIndex];

            int valueIndex = 1;
            string propertyValue = inputs[valueIndex].Trim('"');

            try
            {
                if (propertyName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.firstNameDictionary[propertyValue].AsReadOnly();
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.lastNameDictionary[propertyValue].AsReadOnly();
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParse(propertyValue, out DateTime dob);
                    return this.dateOfBirthDictionary[dob].AsReadOnly();
                }
                else
                {
                    throw new InvalidOperationException($"The {propertyName} isn't valid command searching property. Only " +
                        $"'{nameof(FileCabinetRecord.FirstName)}', '{nameof(FileCabinetRecord.LastName)}' and " +
                        $"'{nameof(FileCabinetRecord.DateOfBirth)}' allowed.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {propertyName} and value {propertyValue} not exist.");
            }
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
