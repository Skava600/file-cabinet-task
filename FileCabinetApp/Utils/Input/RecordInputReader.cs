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
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordInputReader"/> class.
        /// </summary>
        /// <param name="recordValidator"> Record validator. </param>
        public RecordInputReader(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Reads users record input.
        /// </summary>
        /// <returns> Data for FileCabinetRecord. </returns>
        public RecordData GetRecordInput()
        {
            Func<string, Tuple<bool, string, string>> stringConverter = InputConverter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = InputConverter.DateTimeConverter;
            Func<string, Tuple<bool, string, char>> charConverter = InputConverter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = InputConverter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = InputConverter.DecimalConverter;

            Func<string, Tuple<bool, string>> firstNameValidator = this.recordValidator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = this.recordValidator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = this.recordValidator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> sexValidator = this.recordValidator.SexValidator;
            Func<short, Tuple<bool, string>> heightValidator = this.recordValidator.HeightValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = this.recordValidator.SalaryValidator;

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

            RecordData record = new RecordData(firstName, lastName, dateOfBirth, sex, height, salary);
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
