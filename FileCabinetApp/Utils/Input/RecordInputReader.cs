using FileCabinetApp.Converters;
using FileCabinetApp.Models;

namespace FileCabinetApp.Utils.Input
{
    /// <summary>
    /// Record input reader.
    /// </summary>
    public static class RecordInputReader
    {
        /// <summary>
        /// Reads users record input.
        /// </summary>
        /// <returns> Data for FileCabinetRecord. </returns>
        public static RecordData GetRecordInput()
        {
            Func<string, Tuple<bool, string, string>> stringConverter = InputConverter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = InputConverter.DateTimeConverter;
            Func<string, Tuple<bool, string, char>> charConverter = InputConverter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = InputConverter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = InputConverter.DecimalConverter;

            Func<string, Tuple<bool, string>> firstNameValidator = Program.RecordValidator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = Program.RecordValidator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = Program.RecordValidator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> sexValidator = Program.RecordValidator.SexValidator;
            Func<short, Tuple<bool, string>> heightValidator = Program.RecordValidator.HeightValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = Program.RecordValidator.SalaryValidator;

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
