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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream"> Provides a stream for a service. </param>
        /// <param name="validator"> Validator for a service. </param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordData recordData)
        {
            var record = new FileCabinetRecord
            {
                Id = ((int)this.fileStream.Length / RecordSize) + 1,
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Sex = char.ToUpper(recordData.Sex),
                Height = recordData.Height,
                Salary = recordData.Salary,
            };

            this.fileStream.Seek(0, SeekOrigin.End);
            this.WriteRecordToStream(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException("Id can't be less zero");
            }

            if (!this.IsRecordExists(id))
            {
                throw new ArgumentOutOfRangeException($"#{id} record is not found");
            }

            FileCabinetRecord record = new FileCabinetRecord();
            int recordIndex = 0;
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                binaryReader.BaseStream.Seek(2, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    int currentId = binaryReader.ReadInt32();
                    if (currentId == id)
                    {
                        record = new FileCabinetRecord()
                        {
                            Id = currentId,
                            FirstName = recordData.FirstName,
                            LastName = recordData.LastName,
                            DateOfBirth = recordData.DateOfBirth,
                            Sex = recordData.Sex,
                            Height = recordData.Height,
                            Salary = recordData.Salary,
                        };
                        break;
                    }

                    this.fileStream.Seek(RecordSize - sizeof(int), SeekOrigin.Current);
                    recordIndex++;
                }
            }

            this.fileStream.Seek(RecordSize * recordIndex, SeekOrigin.Begin);
            this.WriteRecordToStream(record);
            this.fileStream.Seek(0, SeekOrigin.End);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstname)
        {
            const int startPosition = sizeof(short) + sizeof(int);
            const int propertyFieldSize = NameFieldSize;

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    string currentFirstName = Encoding.Unicode.GetString(binaryReader.ReadBytes(propertyFieldSize)).Trim();
                    if (currentFirstName.Equals(firstname, StringComparison.InvariantCultureIgnoreCase))
                    {
                        binaryReader.BaseStream.Seek(-(propertyFieldSize + startPosition), SeekOrigin.Current);
                        records.Add(this.ReadRecordFromStream(binaryReader));
                        binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Current);
                    }
                    else
                    {
                        binaryReader.BaseStream.Seek(RecordSize - propertyFieldSize, SeekOrigin.Current);
                    }
                }
            }

            this.fileStream.Seek(0, SeekOrigin.End);
            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {firstname} first name not exist.");
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            const int startPosition = sizeof(short)
                    + sizeof(int)
                    + NameFieldSize;
            const int propertyFieldSize = NameFieldSize;

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    string currentLastName = Encoding.Unicode.GetString(binaryReader.ReadBytes(propertyFieldSize)).Trim();
                    if (currentLastName.Equals(lastname, StringComparison.InvariantCultureIgnoreCase))
                    {
                        binaryReader.BaseStream.Seek(-(propertyFieldSize + startPosition), SeekOrigin.Current);
                        records.Add(this.ReadRecordFromStream(binaryReader));
                        binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Current);
                    }
                    else
                    {
                        binaryReader.BaseStream.Seek(RecordSize - propertyFieldSize, SeekOrigin.Current);
                    }
                }
            }

            this.fileStream.Seek(0, SeekOrigin.End);
            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {lastname} last name not exist.");
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            const int startPosition = sizeof(short)
                   + sizeof(int)
                   + (NameFieldSize * 2);
            const int propertyFieldSize = sizeof(int) * 3;

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            DateTime dob;
            try
            {
                dob = DateTime.Parse(dateOfBirth, CultureInfo.InvariantCulture);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException($"Date of birth can't be null.");
            }
            catch (FormatException)
            {
                throw new ArgumentException($"{dateOfBirth} is invalid date format.");
            }

            using (var binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
                while (binaryReader.PeekChar() > -1)
                {
                    DateTime currentDateOfBirth = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    if (currentDateOfBirth.Equals(dob))
                    {
                        binaryReader.BaseStream.Seek(-(propertyFieldSize + startPosition), SeekOrigin.Current);
                        records.Add(this.ReadRecordFromStream(binaryReader));
                        binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Current);
                    }
                    else
                    {
                        binaryReader.BaseStream.Seek(RecordSize - propertyFieldSize, SeekOrigin.Current);
                    }
                }
            }

            this.fileStream.Seek(0, SeekOrigin.End);
            if (records.Count == 0)
            {
                throw new ArgumentException($"Records with {dateOfBirth} date of birth not exist.");
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
                    records.Add(this.ReadRecordFromStream(binaryReader));
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / RecordSize);
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Position = 2;
                while (binaryReader.PeekChar() > -1)
                {
                    if (binaryReader.ReadInt32() == id)
                    {
                        return true;
                    }

                    this.fileStream.Position += RecordSize - sizeof(int);
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
                    this.validator.ValidateParameters(recordData);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Record with id {record.Id} didn't complete validation with message: {ex.Message}");
                    continue;
                }

                if (this.IsRecordExists(record.Id))
                {
                    this.EditRecord(record.Id, recordData);
                }
                else
                {
                    this.CreateRecord(recordData);
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
            binaryReader.ReadInt16();
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
    }
}
