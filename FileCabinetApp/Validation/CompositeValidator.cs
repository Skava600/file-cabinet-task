using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validation
{
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        public void ValidateParameters(RecordData record)
        {
            foreach (IRecordValidator validator in this.validators)
            {
                validator.ValidateParameters(record);
            }
        }
    }
}
