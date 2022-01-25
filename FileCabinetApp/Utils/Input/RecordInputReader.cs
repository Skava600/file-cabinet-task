using FileCabinetApp.Converters;
using FileCabinetApp.Models;
using FileCabinetApp.Validation;

namespace FileCabinetApp.Utils.Input
{
    /// <summary>
    /// Record input reader.
    /// </summary>
    public class RecordInputReader
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinHeight = 0;
        private const short MaxHeight = 300;

        private const decimal MinSalary = 0;
        private const decimal MaxSalary = decimal.MaxValue;

        private static DateTime MinDate => new DateTime(1900, 1, 1);

        private static DateTime MaxDate => DateTime.Now;

        /// <summary>
        /// Reads users record input.
        /// </summary>
        /// <returns> Data for FileCabinetRecord. </returns>
        public RecordData GetRecordInput()
        {
            RecordData record;

            Func<string, Tuple<bool, string, string>> stringConverter = InputConverter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = InputConverter.DateTimeConverter;
            Func<string, Tuple<bool, string, char>> charConverter = InputConverter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = InputConverter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = InputConverter.DecimalConverter;

            Func<string, Tuple<bool, string>> firstNameValidator =
                name => name.Length < MinNameLength || name.Length > MaxNameLength ?
                new Tuple<bool, string>(false, $"Length of first name must be between {MinNameLength} and {MaxNameLength}") :
                new Tuple<bool, string>(true, nameof(record.FirstName));
            Func<string, Tuple<bool, string>> lastNameValidator =
                name => name.Length < MinNameLength || name.Length > MaxNameLength ?
                new Tuple<bool, string>(false, $"Length of last name must be between {MinNameLength} and {MaxNameLength}") :
                new Tuple<bool, string>(true, nameof(record.LastName));
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator =
                dateOfBirth => dateOfBirth < MinDate || dateOfBirth > MaxDate ?
                new Tuple<bool, string>(false, $"Date of birth current must be between {MinDate.ToShortDateString} and {MaxDate.ToShortDateString}") :
                new Tuple<bool, string>(true, nameof(record.DateOfBirth));
            Func<char, Tuple<bool, string>> sexValidator =
                sex => !char.ToUpper(sex).Equals('M') && !char.ToUpper(sex).Equals('F') ?
                new Tuple<bool, string>(false, "sex is only M(male) and F(female)") :
                new Tuple<bool, string>(true, nameof(record.Sex));
            Func<short, Tuple<bool, string>> heightValidator =
                height => height < MinHeight || height > MaxHeight ?
                new Tuple<bool, string>(false, $"height must be a number between {MinHeight}  and {MaxHeight}") :
                new Tuple<bool, string>(true, nameof(record.Height));
            Func<decimal, Tuple<bool, string>> salaryValidator =
                salary => salary < MinSalary || salary > MaxSalary ?
                new Tuple<bool, string>(false, $"Salary should be between {MinSalary} and {MaxSalary}.") :
                new Tuple<bool, string>(true, nameof(record.Salary));

            Console.Write("First name: ");
            var firstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            var lastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);

            Console.Write("Sex (M or F): ");
            var sex = ReadInput(charConverter, sexValidator);

            Console.Write("Height: ");
            var height = ReadInput(shortConverter, heightValidator);

            Console.Write("Salary ($): ");
            var salary = ReadInput(decimalConverter, salaryValidator);

            record = new RecordData(firstName, lastName, dateOfBirth, sex, height, salary);
            return record;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input!);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
