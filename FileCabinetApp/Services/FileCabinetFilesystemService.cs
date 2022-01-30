using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class to describe the file cabinet service.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int NameFieldSize = 120;
        private const int RecordSize =
            sizeof(short)
            + sizeof(int)
            + NameFieldSize
            + NameFieldSize
            + sizeof(int)
            + sizeof(int)
            + sizeof(int)
            + sizeof(char)
            + sizeof(short)
            + sizeof(decimal);

        private readonly IRecordValidator validator;
        private readonly FileStream fileStream;
        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string,  List<long>> lastNameDictionary = new Dictionary<string, List<long>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

        private int lastId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream"> Provides a stream for a service. </param>
        /// <param name="validator"> Validator for a service. </param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
            this.lastId = this.GetStat().Item1 > 0 ? this.GetRecords().Max(rec => rec.Id) : 0;

            var records = this.GetRecords();
            foreach (var record in records)
            {
                long offset = this.GetIndexOf(record.Id) * RecordSize;
                this.AddRecordToDictionaries(record, offset);
            }
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

            this.fileStream.Seek(0, SeekOrigin.End);

            this.AddRecordToDictionaries(record, this.fileStream.Position);
            this.WriteRecordToStream(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            if (id < 0)
            {
                throw new ArgumentException("Id can't be less zero");
            }

            if (!this.IsRecordExists(id))
            {
                throw new ArgumentException($"#{id} record is not found");
            }

            this.validator.ValidateParameters(recordData);

            int recordIndex = 0;
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                binaryReader.BaseStream.Seek(2, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    int currentId = binaryReader.ReadInt32();
                    if (currentId == id)
                    {
                        var oldRecord = this.ReadRecordFromStream(binaryReader);

                        var editedRecord = new FileCabinetRecord()
                        {
                            Id = currentId,
                            FirstName = recordData.FirstName,
                            LastName = recordData.LastName,
                            DateOfBirth = recordData.DateOfBirth,
                            Sex = recordData.Sex,
                            Height = recordData.Height,
                            Salary = recordData.Salary,
                        };

                        this.fileStream.Seek(RecordSize * recordIndex, SeekOrigin.Begin);
                        this.RemoveRecordFromDictionaries(oldRecord, this.fileStream.Position);
                        this.AddRecordToDictionaries(editedRecord, this.fileStream.Position);
                        this.WriteRecordToStream(editedRecord);

                        break;
                    }

                    this.fileStream.Seek(RecordSize - sizeof(int), SeekOrigin.Current);
                    recordIndex++;
                }
            }

            this.fileStream.Seek(0, SeekOrigin.End);

        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            int recordIndex = this.GetIndexOf(id);
            if (recordIndex == -1)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }

            long recordOffset = RecordSize * recordIndex;
            this.fileStream.Seek(recordOffset, SeekOrigin.Begin);
            var record = this.ReadRecordFromStream(new BinaryReader(this.fileStream, Encoding.Unicode, true));
            this.RemoveRecordFromDictionaries(record, recordOffset);

            long isDeletedBytePosition = recordOffset + sizeof(byte);

            this.fileStream.Seek(isDeletedBytePosition, SeekOrigin.Begin);
            this.fileStream.WriteByte(1);

            this.fileStream.Seek(0, SeekOrigin.End);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            var records = this.GetRecords();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var record in records)
            {
                this.WriteRecordToStream(record);
            }

            this.fileStream.SetLength(this.fileStream.Position);
            this.lastId = records.Max(rec => rec.Id);
            foreach (var record in records)
            {
                long offset = this.GetIndexOf(record.Id) * RecordSize;
                this.AddRecordToDictionaries(record, offset);
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstname)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                try
                {
                    var recordsOffsets = this.firstNameDictionary[firstname];
                    foreach (var offset in recordsOffsets)
                    {
                        this.fileStream.Seek(offset, SeekOrigin.Begin);
                        records.Add(this.ReadRecordFromStream(binaryReader));
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Records with {firstname} first name not exist.");
                }
            }

            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {firstname} first name not exist.");
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                try
                {
                    var recordsOffsets = this.lastNameDictionary[lastname];
                    foreach (var offset in recordsOffsets)
                    {
                        this.fileStream.Seek(offset, SeekOrigin.Begin);
                        records.Append(this.ReadRecordFromStream(binaryReader));
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Records with {lastname} last name not exist.");
                }
            }

            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {lastname} first name not exist.");
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            DateTime dob;
            try
            {
                dob = DateTime.Parse(dateOfBirth, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"{dateOfBirth} is invalid date format.");
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                try
                {
                    var recordsOffsets = this.dateOfBirthDictionary[dob];
                    foreach (var offset in recordsOffsets)
                    {
                        this.fileStream.Seek(offset, SeekOrigin.Begin);
                        records.Append(this.ReadRecordFromStream(binaryReader));
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Records with {dateOfBirth} date of birth not exist.");
                }
            }

            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {dateOfBirth} first name not exist.");
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Position = 0;
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                while (binaryReader.PeekChar() > -1)
                {
                    try
                    {
                        records.Add(this.ReadRecordFromStream(binaryReader));
                    }
                    catch (ArgumentException)
                    {
                        binaryReader.ReadBytes(RecordSize - sizeof(short));
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            int numberOfRecords = this.GetRecords().Count;
            int numberOfDeletedRecords = (int)(this.fileStream.Length / RecordSize) - numberOfRecords;
            return new Tuple<int, int>(numberOfRecords, numberOfDeletedRecords);
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(1, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    byte isDeleted = binaryReader.ReadByte();
                    int currentId = binaryReader.ReadInt32();
                    if (currentId == id && isDeleted == 0)
                    {
                        return true;
                    }

                    this.fileStream.Seek(RecordSize - sizeof(int) - sizeof(byte), SeekOrigin.Current);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
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

        private void WriteRecordToStream(FileCabinetRecord record)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                binaryWriter.Write(default(short));
                binaryWriter.Write(record.Id);
                binaryWriter.Write(record.FirstName!.PadRight(NameFieldSize / sizeof(char)).ToCharArray());
                binaryWriter.Write(record.LastName!.PadRight(NameFieldSize / sizeof(char)).ToCharArray());
                binaryWriter.Write(record.DateOfBirth.Year);
                binaryWriter.Write(record.DateOfBirth.Month);
                binaryWriter.Write(record.DateOfBirth.Day);
                binaryWriter.Write(record.Sex);
                binaryWriter.Write(record.Height);
                binaryWriter.Write(record.Salary);
            }
        }

        private FileCabinetRecord ReadRecordFromStream(BinaryReader binaryReader)
        {
            binaryReader.ReadByte();
            byte b = binaryReader.ReadByte();
            if (b == 1)
            {
                throw new ArgumentException("This record is deleted.");
            }

            return new FileCabinetRecord
            {
                Id = binaryReader.ReadInt32(),
                FirstName = Encoding.Unicode.GetString(binaryReader.ReadBytes(NameFieldSize)).Trim(),
                LastName = Encoding.Unicode.GetString(binaryReader.ReadBytes(NameFieldSize)).Trim(),
                DateOfBirth = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32()),
                Sex = binaryReader.ReadChar(),
                Height = binaryReader.ReadInt16(),
                Salary = binaryReader.ReadDecimal(),
            };
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

        private int GetIndexOf(int id)
        {
            int index = 0;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(1, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    byte isDeleted = binaryReader.ReadByte();
                    int currentId = binaryReader.ReadInt32();
                    if (currentId == id && isDeleted == 0)
                    {
                        return index;
                    }

                    this.fileStream.Seek(RecordSize - sizeof(int) - sizeof(byte), SeekOrigin.Current);
                    index++;
                }
            }

            return -1;
        }

        private void AddRecordToDictionaries(FileCabinetRecord record, long offset)
        {
            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary.Add(record.FirstName, new List<long>());
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary.Add(record.LastName, new List<long>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<long>());
            }

            this.firstNameDictionary[record.FirstName].Add(offset);
            this.lastNameDictionary[record.LastName].Add(offset);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(offset);
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord record, long offset)
        {
            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                return;
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                return;
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                return;
            }

            this.firstNameDictionary[record.FirstName].Remove(offset);
            this.lastNameDictionary[record.LastName].Remove(offset);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(offset);
        }
    }
}
