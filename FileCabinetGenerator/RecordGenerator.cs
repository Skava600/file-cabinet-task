using System.Text;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class for generating data for <see cref="FileCabinetRecord"/>.
    /// </summary>
    public static class RecordGenerator
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinHeight = 60;
        private const short MaxHeight = 272;

        private static readonly DateTime MinDateOfBirth = new DateTime(1950, 1, 1);

        /// <summary>
        /// This method generates <see cref="FileCabinetRecord"/> with random properties.
        /// </summary>
        /// <param name="id">id of generated record.</param>
        /// <returns>Generated <see cref="FileCabinetRecord"/>.</returns>
        public static RecordSerializable GenerateRecord(int id)
        {
            Random random = new Random();
            RecordSerializable newRecord = new RecordSerializable()
            {
                Id = id,
                Name = new Name()
                {
                    FirstName = GenerateName(random.Next(MinNameLength, MaxNameLength + 1)),
                    LastName = GenerateName(random.Next(MinNameLength, MaxNameLength + 1)),
                },
                DateOfBirth = GenerateDateOfBirth(),
                Sex = GenerateSex(),
                Height = (short)random.Next(MinHeight, MaxHeight + 1),
                Salary = random.Next(),
            };

            return newRecord;
        }

        private static string GenerateName(int length)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            StringBuilder name = new StringBuilder();
            while (name.Length < length)
            {
                name.Append(consonants[r.Next(consonants.Length)]);
                name.Append(vowels[r.Next(vowels.Length)]);
            }

            return name.ToString()[..length];
        }

        private static char GenerateSex()
        {
            var sex = new Random().Next(2);

            return sex == 0 ? 'M' : 'F';
        }

        private static DateTime GenerateDateOfBirth()
        {
            Random gen = new Random();
            int range = (DateTime.Today - MinDateOfBirth).Days;
            return MinDateOfBirth.AddDays(gen.Next(range));
        }
    }
}
