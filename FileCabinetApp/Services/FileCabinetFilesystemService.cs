using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

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

        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream"> Provides a stream for a service. </param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
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

            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                binaryWriter.Write(default(short));
                binaryWriter.Write(record.Id);
                binaryWriter.Write(record.FirstName.PadRight(NameFieldSize / sizeof(char)).ToCharArray());
                binaryWriter.Write(record.LastName.PadRight(NameFieldSize / sizeof(char)).ToCharArray());
                binaryWriter.Write(record.DateOfBirth.Year);
                binaryWriter.Write(record.DateOfBirth.Month);
                binaryWriter.Write(record.DateOfBirth.Day);
                binaryWriter.Write(record.Sex);
                binaryWriter.Write(record.Height);
                binaryWriter.Write(record.Salary);
            }

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByProperty(string property)
        {
            throw new NotImplementedException();
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
                    binaryReader.ReadInt16();
                    records.Add(new FileCabinetRecord
                    {
                        Id = binaryReader.ReadInt32(),
                        FirstName = Encoding.Unicode.GetString(binaryReader.ReadBytes(NameFieldSize)).Trim(),
                        LastName = Encoding.Unicode.GetString(binaryReader.ReadBytes(NameFieldSize)).Trim(),
                        DateOfBirth = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32()),
                        Sex = binaryReader.ReadChar(),
                        Height = binaryReader.ReadInt16(),
                        Salary = binaryReader.ReadDecimal(),
                    });
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
