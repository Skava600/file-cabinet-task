using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
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
            this.lastId = this.records.Count > 0 ? this.GetRecords().Max(rec => rec.Id) : 0;
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

            this.AddRecordToDictionaries(record.FirstName, record.LastName, record.DateOfBirth, record);

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

            this.firstNameDictionary[record.FirstName !].Remove(record);
            this.lastNameDictionary[record.LastName!].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

            record.FirstName = recordData.FirstName;
            record.LastName = recordData.LastName;
            record.DateOfBirth = recordData.DateOfBirth;
            record.Sex = recordData.Sex;
            record.Height = recordData.Height;
            record.Salary = recordData.Salary;

            this.AddRecordToDictionaries(recordData.FirstName, recordData.LastName, recordData.DateOfBirth, record);
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
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.records.Count, 0);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstname)
        {
            try
            {
                return this.firstNameDictionary[firstname].AsReadOnly();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {firstname} first name not exist.");
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            try
            {
                return this.lastNameDictionary[lastName].AsReadOnly();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {lastName} last name not exist.");
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            try
            {
                DateTime dob = DateTime.Parse(dateOfBirth, CultureInfo.InvariantCulture);
                return this.dateOfBirthDictionary[dob].AsReadOnly();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"Records with {dateOfBirth} date of birth not exist.");
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException($"Date of birth can't be null.");
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
