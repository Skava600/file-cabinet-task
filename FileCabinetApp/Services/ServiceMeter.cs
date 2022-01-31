using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        public int CreateRecord(RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int id = this.fileCabinetService.CreateRecord(recordData);
            stopWatch.Stop();
            Console.WriteLine($"Create method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return id;
        }

        public void EditRecord(int id, RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.EditRecord(id, recordData);
            stopWatch.Stop();
            Console.WriteLine($"Edit method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            stopWatch.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstname)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.FindByFirstName(firstname);
            stopWatch.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        public IEnumerable<FileCabinetRecord> FindByLastName(string lastname)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.FindByLastName(lastname);
            stopWatch.Stop();
            Console.WriteLine($"FindByLastName method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var iterator = this.fileCabinetService.GetRecords();
            stopWatch.Stop();
            Console.WriteLine($"GetRecords method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return iterator;
        }

        public Tuple<int, int> GetStat()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var stat = this.fileCabinetService.GetStat();
            stopWatch.Stop();
            Console.WriteLine($"GetStat method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return stat;
        }

        public bool IsRecordExists(int id)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var isExist = this.fileCabinetService.IsRecordExists(id);
            stopWatch.Stop();
            Console.WriteLine($"IsRecordExists method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return isExist;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var snapshot = this.fileCabinetService.MakeSnapshot();
            stopWatch.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return snapshot;
        }

        public void Purge()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.Purge();
            stopWatch.Stop();
            Console.WriteLine($"Purge method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        public void RemoveRecord(int id)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.fileCabinetService.RemoveRecord(id);
            stopWatch.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

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
