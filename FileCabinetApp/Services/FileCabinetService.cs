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
    public class FileCabinetService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary =
            new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <inheritdoc/>
        public int CreateRecord(RecordData recordData)
        {
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

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            FileCabinetRecord record = this.records.Find(rec => rec.Id == id)
                ?? throw new ArgumentOutOfRangeException(nameof(id), $"#{id} record is not found");

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

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.records);
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            return this.records.Exists(record => record.Id == id);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.records.Count;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByProperty(string property)
        {
            string[] inputs = property.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputs.Length < 2)
            {
                throw new InvalidOperationException($"The '{property}' isn't valid command parameters. " +
                    $"Should be name of property and value through white space.");
            }

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

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records.ToArray());
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
