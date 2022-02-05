using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Iterators;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class to describe the file cabinet service.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary =
            new Dictionary<DateTime, List<FileCabinetRecord>>();

        private readonly Dictionary<char, List<FileCabinetRecord>> sexDictionary =
            new Dictionary<char, List<FileCabinetRecord>>();

        private readonly Dictionary<short, List<FileCabinetRecord>> heightDictionary =
            new Dictionary<short, List<FileCabinetRecord>>();

        private readonly Dictionary<decimal, List<FileCabinetRecord>> salaryDictionary =
            new Dictionary<decimal, List<FileCabinetRecord>>();

        private readonly IRecordValidator validator;

        private int lastId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator"><see cref="IRecordValidator"/>.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
            this.lastId = this.records.Count > 0 ? this.records.Max(rec => rec.Id) : 0;
        }

        /// <inheritdoc/>
        public void CreateRecordWithId(int id, RecordData recordData)
        {
            if (id < 1)
            {
                throw new ArgumentException($"Id can't be less than one.");
            }

            if (this.IsRecordExists(id))
            {
                throw new ArgumentException($"Record with id {id} is already existing.");
            }

            this.validator.ValidateParameters(recordData);

            if (id > this.lastId)
            {
                this.lastId = id;
            }

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Sex = char.ToUpper(recordData.Sex),
                Height = recordData.Height,
                Salary = recordData.Salary,
            };

            this.records.Add(record);

            this.AddRecordToDictionaries(record);
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordData recordData)
        {
            this.validator.ValidateParameters(recordData);
            var record = new FileCabinetRecord
            {
                Id = this.GenerateId(),
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Sex = char.ToUpper(recordData.Sex),
                Height = recordData.Height,
                Salary = recordData.Salary,
            };

            this.records.Add(record);

            this.AddRecordToDictionaries(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            if (id < 0)
            {
                throw new ArgumentException("Id can't be less zero");
            }

            this.validator.ValidateParameters(recordData);

            FileCabinetRecord record = this.records.Find(rec => rec.Id == id)
                ?? throw new ArgumentException($"#{id} record is not found");

            this.RemoveRecordFromDictionaries(record);

            record.FirstName = recordData.FirstName;
            record.LastName = recordData.LastName;
            record.DateOfBirth = recordData.DateOfBirth;
            record.Sex = recordData.Sex;
            record.Height = recordData.Height;
            record.Salary = recordData.Salary;

            this.AddRecordToDictionaries(record);
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            if (!this.IsRecordExists(id))
            {
                Console.WriteLine($"#{id} record is not found.");
            }

            var record = this.records.Find(rec => rec.Id == id);

            if (record != null)
            {
                this.records.Remove(record);
                this.RemoveRecordFromDictionaries(record);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<int> DeleteRecord(PropertyInfo propertyInfo, string propertyValue)
        {
            var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
            var value = converter.ConvertFromString(propertyValue);
            if (value == null)
            {
                throw new ArgumentException($"Wrong property value: {propertyValue}");
            }

            IEnumerable<FileCabinetRecord> records;

            records = this.FindByProperty(propertyInfo, propertyValue);

            List<int> deletedRecordsIds = new List<int>();
            foreach (var record in records)
            {
                this.records.Remove(record);
                this.RemoveRecordFromDictionaries(record);
                deletedRecordsIds.Add(record.Id);
            }

            return deletedRecordsIds;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            return this.records;
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            return this.records.Exists(record => record.Id == id);
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.records.Count, 0);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByProperty(PropertyInfo propertyInfo, string propertyValue)
        {
            IEnumerable<FileCabinetRecord> records;
            try
            {
                switch (propertyInfo.Name)
                {
                    case nameof(FileCabinetRecord.Id):
                        int id = int.Parse(propertyValue);
                        records = this.records.Where(rec => rec.Id == id);
                        break;
                    case nameof(FileCabinetRecord.FirstName):
                        records = this.firstNameDictionary[propertyValue];
                        break;
                    case nameof(FileCabinetRecord.LastName):
                        records = this.lastNameDictionary[propertyValue];
                        break;
                    case nameof(FileCabinetRecord.DateOfBirth):
                        var dob = DateTime.Parse(propertyValue, CultureInfo.InvariantCulture);
                        records = this.dateOfBirthDictionary[dob];
                        break;
                    case nameof(FileCabinetRecord.Sex):
                        var sex = char.ToUpper(char.Parse(propertyValue));
                        records = this.sexDictionary[sex];
                        break;
                    case nameof(FileCabinetRecord.Height):
                        var height = short.Parse(propertyValue);
                        records = this.heightDictionary[height];
                        break;
                    case nameof(FileCabinetRecord.Salary):
                        var salary = decimal.Parse(propertyValue);
                        records = this.salaryDictionary[salary];
                        break;
                    default:
                        throw new ArgumentException($"There is no such property as : {propertyInfo.Name} or it is not supported");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {propertyValue} {propertyInfo.Name} not exist");
            }
            catch (FormatException)
            {
                throw new ArgumentException($"{propertyValue} is invalid {propertyInfo.PropertyType} format");
            }

            return records;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records.ToArray());
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            foreach (var record in snapshot.Records)
            {
                var recordData = new RecordData(record);

                try
                {
                    if (this.IsRecordExists(record.Id))
                    {
                        this.EditRecord(record.Id, recordData);
                    }
                    else
                    {
                        this.CreateRecord(recordData);
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Record with id {record.Id} didn't complete validation with message: {ex.Message}");
                    continue;
                }
            }
        }

        private void AddRecordToDictionaries(FileCabinetRecord record)
        {
            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
            }

            if (!this.sexDictionary.ContainsKey(record.Sex))
            {
                this.sexDictionary.Add(record.Sex, new List<FileCabinetRecord>());
            }

            if (!this.heightDictionary.ContainsKey(record.Height))
            {
                this.heightDictionary.Add(record.Height, new List<FileCabinetRecord>());
            }

            if (!this.salaryDictionary.ContainsKey(record.Salary))
            {
                this.salaryDictionary.Add(record.Salary, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[record.FirstName].Add(record);
            this.lastNameDictionary[record.LastName].Add(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
            this.sexDictionary[record.Sex].Add(record);
            this.heightDictionary[record.Height].Add(record);
            this.salaryDictionary[record.Salary].Add(record);
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Remove(record);
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Remove(record);
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
            }

            if (this.sexDictionary.ContainsKey(record.Sex))
            {
                this.sexDictionary[record.Sex].Remove(record);
            }

            if (this.heightDictionary.ContainsKey(record.Height))
            {
                this.heightDictionary[record.Height].Remove(record);
            }

            if (this.salaryDictionary.ContainsKey(record.Salary))
            {
                this.salaryDictionary[record.Salary].Remove(record);
            }
        }

        private int GenerateId()
        {
            int id = this.lastId != int.MaxValue ? this.lastId : 0;

            while (++id != int.MinValue)
            {
                if (!this.IsRecordExists(id))
                {
                    this.lastId = id;
                    return id;
                }
            }

            throw new ArgumentException("All ids are occupied.");
        }
    }
}
