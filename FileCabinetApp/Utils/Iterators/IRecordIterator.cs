using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.Iterators
{
    public interface IRecordIterator
    {
        FileCabinetRecord GetNext();

        bool HasMore();
    }
}
