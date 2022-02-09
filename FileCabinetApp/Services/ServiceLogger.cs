using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Iterators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Service logger.
    /// </summary>
    internal class ServiceLogger : IFileCabinetService, IDisposable
    {
        private const string LoggerFile = "logs.txt";

        private TextWriter writer;

        private IFileCabinetService fileCabinetService;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        public ServiceLogger(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;

            this.writer = new StreamWriter(File.Create(LoggerFile));
        }

        /// <inheritdoc/>
        public void CreateRecordWithId(int id, RecordData recordData)
        {
            this.Log($"Calling {nameof(this.CreateRecordWithId)}() with Id = '{id}', FirstName = '{recordData.FirstName}'," +
                $"LastName = '{recordData.LastName}', " +
                $"DateOfBirth = {recordData.DateOfBirth}, " +
                $"Sex = '{recordData.Sex}', " +
                $"Height = '{recordData.Height}', " +
                $"Salary = '{recordData.Salary}'");
            try
            {
                this.fileCabinetService.CreateRecordWithId(id, recordData);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.CreateRecord)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.CreateRecordWithId)}() returned '{id}'");
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordData recordData)
        {
            this.Log($"Calling {nameof(this.CreateRecord)}() with FirstName = '{recordData.FirstName}'," +
                $"LastName = '{recordData.LastName}', " +
                $"DateOfBirth = {recordData.DateOfBirth}, " +
                $"Sex = '{recordData.Sex}', " +
                $"Height = '{recordData.Height}', " +
                $"Salary = '{recordData.Salary}'");
            int id;
            try
            {
                id = this.fileCabinetService.CreateRecord(recordData);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.CreateRecord)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.CreateRecord)}() returned '{id}'");
            return id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            this.Log($"Calling {nameof(this.EditRecord)}() with FirstName = {recordData.FirstName}," +
                $"LastName = {recordData.LastName}, " +
                $"DateOfBirth = {recordData.DateOfBirth}, " +
                $"Sex = {recordData.Sex}, " +
                $"Height = {recordData.Height}, " +
                $"Salary = {recordData.Salary}");

            try
            {
                this.fileCabinetService.EditRecord(id, recordData);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.EditRecord)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.EditRecord)}() returned successfuly");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByProperty(PropertyInfo propertyInfo, string propertyValue)
        {
            this.Log($"Calling {nameof(this.FindByProperty)}() with {propertyInfo.Name} = '{propertyValue}'");
            IEnumerable<FileCabinetRecord> iterator;
            try
            {
                iterator = this.fileCabinetService.FindByProperty(propertyInfo, propertyValue);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.FindByProperty)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.FindByProperty)}() returned successsfuly.");
            return iterator;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.Log($"Calling {nameof(this.GetRecords)}()");
            var iterator = this.fileCabinetService.GetRecords();
            this.Log($"{nameof(this.GetRecords)}() returned successfully.");
            return iterator;
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            this.Log($"Calling {nameof(this.GetStat)}()");
            var stat = this.fileCabinetService.GetStat();
            this.Log($"{nameof(this.GetStat)}() returned '({stat.Item1}, {stat.Item2})'");
            return stat;
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            this.Log($"Calling {nameof(this.IsRecordExists)}() with id = '{id}'");
            var isExists = this.fileCabinetService.IsRecordExists(id);
            this.Log($"{nameof(this.IsRecordExists)}() returned '{isExists}'");
            return isExists;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Log($"Calling {nameof(this.MakeSnapshot)}()");
            var snapshot = this.fileCabinetService.MakeSnapshot();
            this.Log($"{nameof(this.IsRecordExists)}() returned snapshot successfuly");
            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.Log($"Calling {nameof(this.Purge)}()");
            this.fileCabinetService.Purge();
            this.Log($"{nameof(this.Purge)}() returned successfuly");
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            this.Log($"Calling {nameof(this.RemoveRecord)}() with id = '{id}'");
            try
            {
                this.fileCabinetService.RemoveRecord(id);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.RemoveRecord)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.RemoveRecord)}() returned successfuly");
        }

        /// <inheritdoc/>
        public IEnumerable<int> DeleteRecord(PropertyInfo propertyInfo, string propertyValue)
        {
            IEnumerable<int> deletedRecordsIds;
            this.Log($"Calling {nameof(this.DeleteRecord)}() with {propertyInfo.Name} = {propertyValue}");
            try
            {
                deletedRecordsIds = this.fileCabinetService.DeleteRecord(propertyInfo, propertyValue);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.DeleteRecord)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.DeleteRecord)}() returned successfuly");
            return deletedRecordsIds;
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.Log($"Calling {nameof(this.Restore)}() with {snapshot.Records.Count} records");
            try
            {
                this.fileCabinetService.Restore(snapshot);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.Restore)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.Restore)}() returned successfuly");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases sevice logger's unmanaged resourses.
        /// </summary>
        /// <param name="disposing"> Dispose or not. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.writer.Dispose();
                }

                this.disposedValue = true;
            }
        }

        private void Log(string message)
        {
            this.writer.WriteLine($"{DateTime.Now} - {message}.");
            this.writer.Flush();
        }
    }
}