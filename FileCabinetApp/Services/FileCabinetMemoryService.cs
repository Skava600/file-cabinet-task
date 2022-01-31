using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

            FileCabinetRecord record = this.records.Find(rec => rec.Id == id)
                ?? throw new ArgumentException($"#{id} record is not found");

            this.validator.ValidateParameters(recordData);

            this.firstNameDictionary[record.FirstName].Remove(record);
            this.lastNameDictionary[record.LastName].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

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

            this.records.RemoveAll(rec => rec.Id == id);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IRecordIterator GetRecords()
        {
            return new MemoryIterator(this.records);
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
        public IRecordIterator FindByFirstName(string firstname)
        {
            try
            {
                return new MemoryIterator(this.firstNameDictionary[firstname]);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {firstname} first name not exist.");
            }
        }

        /// <inheritdoc/>
        public IRecordIterator FindByLastName(string lastName)
        {
            try
            {
                return new MemoryIterator(this.lastNameDictionary[lastName]);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {lastName} last name not exist.");
            }
        }

        /// <inheritdoc/>
        public IRecordIterator FindByDateOfBirth(string dateOfBirth)
        {
            try
            {
                DateTime dob = DateTime.Parse(dateOfBirth, CultureInfo.InvariantCulture);
                return new MemoryIterator(this.dateOfBirthDictionary[dob]);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {dateOfBirth} date of birth not exist.");
            }
            catch (FormatException)
            {
                throw new ArgumentException($"{dateOfBirth} is invalid date format.");
            }
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

            this.firstNameDictionary[record.FirstName].Add(record);
            this.lastNameDictionary[record.LastName].Add(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
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
