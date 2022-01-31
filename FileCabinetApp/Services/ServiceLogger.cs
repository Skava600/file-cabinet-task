using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Iterators;

namespace FileCabinetApp.Services
{
    internal class ServiceLogger : IFileCabinetService, IDisposable
    {
        private const string LoggerFile = "logs.txt";

        private TextWriter writer;

        private IFileCabinetService fileCabinetService;
        private bool disposedValue;

        public ServiceLogger(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;

            this.writer = new StreamWriter(File.Create(LoggerFile));
        }

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

        public IRecordIterator FindByDateOfBirth(string dateOfBirth)
        {
            this.Log($"Calling {nameof(this.FindByDateOfBirth)}() with DateOfBirth = '{dateOfBirth}'");
            IRecordIterator iterator;
            try
            {
                iterator = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.FindByDateOfBirth)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.FindByDateOfBirth)}() returned successsfuly.");
            return iterator;
        }

        public IRecordIterator FindByFirstName(string firstname)
        {
            this.Log($"Calling {nameof(this.FindByFirstName)}() with firstname = '{firstname}'");
            IRecordIterator iterator;
            try
            {
                iterator = this.fileCabinetService.FindByFirstName(firstname);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.FindByFirstName)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.FindByFirstName)}() returned successfully.");
            return iterator;
        }

        public IRecordIterator FindByLastName(string lastname)
        {
            this.Log($"Calling {nameof(this.FindByLastName)}() with lastname = '{lastname}'");
            IRecordIterator iterator;
            try
            {
                iterator = this.fileCabinetService.FindByLastName(lastname);
            }
            catch (Exception ex)
            {
                this.Log($"{nameof(this.fileCabinetService.FindByLastName)}() finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            this.Log($"{nameof(this.FindByLastName)}() returned successfully.");
            return iterator;
        }

        public IRecordIterator GetRecords()
        {
            this.Log($"Calling {nameof(this.GetRecords)}()");
            var iterator = this.fileCabinetService.GetRecords();
            this.Log($"{nameof(this.GetRecords)}() returned successfully.");
            return iterator;
        }

        public Tuple<int, int> GetStat()
        {
            this.Log($"Calling {nameof(this.GetStat)}()");
            var stat = this.fileCabinetService.GetStat();
            this.Log($"{nameof(this.GetStat)}() returned '({stat.Item1}, {stat.Item2})'");
            return stat;
        }

        public bool IsRecordExists(int id)
        {
            this.Log($"Calling {nameof(this.IsRecordExists)}() with id = '{id}'");
            var isExists = this.fileCabinetService.IsRecordExists(id);
            this.Log($"{nameof(this.IsRecordExists)}() returned '{isExists}'");
            return isExists;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Log($"Calling {nameof(this.MakeSnapshot)}()");
            var snapshot = this.fileCabinetService.MakeSnapshot();
            this.Log($"{nameof(this.IsRecordExists)}() returned snapshot successfuly");
            return snapshot;
        }

        public void Purge()
        {
            this.Log($"Calling {nameof(this.Purge)}()");
            this.fileCabinetService.Purge();
            this.Log($"{nameof(this.Purge)}() returned successfuly");
        }

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

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

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