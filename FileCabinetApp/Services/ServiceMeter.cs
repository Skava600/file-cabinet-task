using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Iterators;

namespace FileCabinetApp.Services
{
    internal class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService fileCabinetService;

        public ServiceMeter(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int id = this.fileCabinetService.CreateRecord(recordData);
            stopWatch.Stop();
            Console.WriteLine($"Create method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return id;
        }

        /// <inheritdoc/>
        public void CreateRecordWithId(int id, RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.CreateRecordWithId(id, recordData);
            stopWatch.Stop();
            Console.WriteLine($"Insert method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <inheritdoc/>
        public IEnumerable<int> DeleteRecord(PropertyInfo propertyInfo, string propertyValue)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var deletedRecordIds = this.fileCabinetService.DeleteRecord(propertyInfo, propertyValue);
            stopWatch.Stop();
            Console.WriteLine($"DeleteRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return deletedRecordIds;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.EditRecord(id, recordData);
            stopWatch.Stop();
            Console.WriteLine($"Edit method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByProperty(PropertyInfo propertyInfo, string propertyValue)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.FindByProperty(propertyInfo, propertyValue);
            stopWatch.Stop();
            Console.WriteLine($"FindByProperty method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.GetRecords();
            stopWatch.Stop();
            Console.WriteLine($"GetRecords method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var stat = this.fileCabinetService.GetStat();
            stopWatch.Stop();
            Console.WriteLine($"GetStat method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return stat;
        }

        /// <inheritdoc/>
        public bool IsRecordExists(int id)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var isExist = this.fileCabinetService.IsRecordExists(id);
            stopWatch.Stop();
            Console.WriteLine($"IsRecordExists method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return isExist;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var snapshot = this.fileCabinetService.MakeSnapshot();
            stopWatch.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.Purge();
            stopWatch.Stop();
            Console.WriteLine($"Purge method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.RemoveRecord(id);
            stopWatch.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.Restore(snapshot);
            stopWatch.Stop();
            Console.WriteLine($"Restore method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }
    }
}
