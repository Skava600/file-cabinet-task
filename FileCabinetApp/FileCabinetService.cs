namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            ValidateInfo(firstName, lastName, dateOfBirth, sex, height, salary);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                Height = height,
                Salary = salary,
            };

            this.list.Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            FileCabinetRecord record = this.list.Find(rec => rec.Id == id)
                ?? throw new ArgumentOutOfRangeException(nameof(id), $"#{id} record is not found");

            ValidateInfo(firstName, lastName, dateOfBirth, sex, height, salary);

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Sex = sex;
            record.Height = height;
            record.Salary = salary;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public bool IsRecordExists(int id)
        {
            return this.list.Exists(record => record.Id == id);
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private static void ValidateInfo(string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Length of first Name must be between 2 and 60.", nameof(firstName));
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Length of last Name must be between 2 and 60.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth is not valid.", nameof(dateOfBirth));
            }

            sex = char.ToUpper(sex);
            if (!sex.Equals('M') && !sex.Equals('F'))
            {
                throw new ArgumentException("sex is only M(male) and F(female).", nameof(sex));
            }

            if (height < 60 || height > 272)
            {
                throw new ArgumentException("height value isn't valid.", nameof(height));
            }

            if (salary < 0)
            {
                throw new ArgumentException("salary can't be less zero.", nameof(salary));
            }
        }
    }
}
