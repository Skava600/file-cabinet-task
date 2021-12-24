namespace FileCabinetApp
{
    /// <summary>
    /// Class to describe the file cabinet service.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary =
            new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary =
            new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>This method creates new FileCabinetRecord with given properties.</summary>
        /// <param name="firstName">the first name of new record.</param>
        /// <param name="lastName">the last name of new record.</param>
        /// <param name="dateOfBirth">date of birth of new record.</param>
        /// <param name="sex">sex of a new record.</param>
        /// <param name="height">height of new record.</param>
        /// <param name="salary">salary of new record.</param>
        /// <returns>return a number representing id of the new record.</returns>
        public int CreateRecord(string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            ValidateRecordParams(firstName, lastName, dateOfBirth, sex, height, salary);
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

            this.AddRecordToDictionaries(firstName!, lastName!, dateOfBirth, record);

            return record.Id;
        }

        /// <summary>This method edites FileCabinetRecord found by id with given properties.</summary>
        /// <param name="id">ID of editing record.</param>
        /// <param name="firstName">new value of first name.</param>
        /// <param name="lastName">new value of last name.</param>
        /// <param name="dateOfBirth">new value of date of birth.</param>
        /// <param name="sex">new value of sex.</param>
        /// <param name="height">new value of height.</param>
        /// <param name="salary">new value of salary .</param>
        public void EditRecord(int id, string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            FileCabinetRecord record = this.list.Find(rec => rec.Id == id)
                ?? throw new ArgumentOutOfRangeException(nameof(id), $"#{id} record is not found");

            ValidateRecordParams(firstName, lastName, dateOfBirth, sex, height, salary);

            this.firstNameDictionary[record.FirstName !].Remove(record);
            this.lastNameDictionary[record.LastName!].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
            this.AddRecordToDictionaries(firstName!, lastName!, dateOfBirth, record);

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Sex = sex;
            record.Height = height;
            record.Salary = salary;
        }

        /// <summary>This method for getting array of records.</summary>
        /// <returns>array of registered FileCabinetRecord.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>This method checks if the record with given id exists.</summary>
        /// <param name="id">id of record.</param>
        /// <returns><c>true</c> if record exists and <c>false</c> otherwise.</returns>
        public bool IsRecordExists(int id)
        {
            return this.list.Exists(record => record.Id == id);
        }

        /// <summary>This method for getting quantity of registered records.</summary>
        /// <returns>int number of reg.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Finds the specified first name.
        /// </summary>
        /// <param name="firstName">The first name of record.</param>
        /// <returns>All records with specified first name.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the specified last name.
        /// </summary>
        /// <param name="lastName">The last name of record.</param>
        /// <returns>All records with specified last name.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth of record.</param>
        /// <returns>All records with specified date of birth.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime.TryParse(dateOfBirth, out DateTime dob);
            if (this.dateOfBirthDictionary.ContainsKey(dob))
            {
                return this.dateOfBirthDictionary[dob].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        private static void ValidateRecordParams(string? firstName, string? lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
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

            DateTime minDate = new DateTime(1950, 1, 1);
            if (dateOfBirth < minDate || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth must be between 1 Jan 1950 and current date.", nameof(dateOfBirth));
            }

            sex = char.ToUpper(sex);
            if (!sex.Equals('M') && !sex.Equals('F'))
            {
                throw new ArgumentException("sex is only M(male) and F(female).", nameof(sex));
            }

            if (height < 60 || height > 272)
            {
                throw new ArgumentException("height must be a number between 60 and 272.", nameof(height));
            }

            if (salary < 0)
            {
                throw new ArgumentException("salary can't be less zero.", nameof(salary));
            }
        }

        private void AddRecordToDictionaries(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord record)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstName].Add(record);
            this.lastNameDictionary[lastName].Add(record);
            this.dateOfBirthDictionary[dateOfBirth].Add(record);
        }
    }
}
