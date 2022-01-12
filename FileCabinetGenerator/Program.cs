using System;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Services;
using FileCabinetApp.Utils.Enums;
using FileCabinetGenerator;

/// <summary>
/// The program class.
/// </summary>
public static class Program
{
    private static readonly Dictionary<string, Action<string>> CommandParameters = new Dictionary<string, Action<string>>
    {
        ["--output-type"] = (string outputType) => Program.outputType = outputType,
        ["-t"] = (string outputType) => Program.outputType = outputType,
        ["--output"] = (string output) => Program.outputFileName = output,
        ["-o"] = (string output) => Program.outputFileName = output,
        ["--records-amount"] = (string recordsAmount) => Program.recordsAmount = recordsAmount,
        ["-a"] = (string recordsAmount) => Program.recordsAmount = recordsAmount,
        ["--start-id"] = (string startId) => Program.startId = startId,
        ["-i"] = (string startId) => Program.startId = startId,
    };

    private static string? outputType;
    private static string? outputFileName;
    private static string? recordsAmount;
    private static string? startId;

    /// <summary>
    /// Entry point of the application.
    /// </summary>
    /// <param name="args">The arguments of application.</param>
    public static void Main(string[] args)
    {
        string paramName;
        string paramValue;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--"))
            {
                string[] param = args[i].Split('=', 2);
                const int paramIndex = 0;
                const int paramValueIndex = 1;
                if (param.Length < 2)
                {
                    continue;
                }

                paramName = param[paramIndex];
                paramValue = param[paramValueIndex];
            }
            else if (args[i].StartsWith('-') && i + 1 < args.Length)
            {
                paramName = args[i];
                paramValue = args[i + 1];
                i++;
            }
            else
            {
                continue;
            }

            Action<string>? setParameter;
            if (CommandParameters.TryGetValue(paramName, out setParameter))
            {
                setParameter(paramValue);
            }
            else
            {
                Console.WriteLine($"error: unknown parameter '{paramName}'");
                return;
            }
        }

        try
        {
            ValidateCommandParameters();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        List<RecordSerializable> generatedRecords = GenerateRecords(int.Parse(startId!), int.Parse(recordsAmount!));
        Export(generatedRecords);
    }

    private static List<RecordSerializable> GenerateRecords(int startId, int recordsAmount)
    {
        List<RecordSerializable> generatedRecords = new List<RecordSerializable>();

        for (int i = startId; i < recordsAmount + startId; i++)
        {
            generatedRecords.Add(RecordGenerator.GenerateRecord(i));
        }

        return generatedRecords;
    }

    private static void Export(List<RecordSerializable> generatedRecords)
    {
        if (File.Exists(outputFileName))
        {
            char answer;

            do
            {
                Console.Write($"File is exist - rewrite {outputFileName}? [Y/n] ");
                answer = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }
            while (!char.ToLower(answer).Equals('y') && !char.ToLower(answer).Equals('n'));

            if (char.ToLower(answer).Equals('n'))
            {
                return;
            }
        }

        try
        {
            if (outputType!.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                using (StreamWriter sw = new StreamWriter(outputFileName!))
                {
                    foreach (var record in generatedRecords)
                    {
                        sw.WriteLine(record.ToString());
                    }
                }
            }
            else if (outputType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                };
                using (var fileWriter = XmlWriter.Create(outputFileName!, settings))
                {
                    var serializableRecords = new RecordsSerializable(generatedRecords);

                    var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                    xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                    var serializer = new XmlSerializer(typeof(RecordsSerializable));
                    serializer.Serialize(fileWriter, serializableRecords, xmlSerializerNamespaces);
                }
            }
            else
            {
                throw new ArgumentException($"{outputType} is not correct format, available only xml and csv.");
            }

            Console.WriteLine($"{generatedRecords.Count} records were written to {outputFileName}.");
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            Console.WriteLine($"Export failed: can't open file {outputFileName}.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void ValidateCommandParameters()
    {
        if (outputType == null)
        {
            throw new ArgumentException("error: output type not set.");
        }

        if (outputFileName == null)
        {
            throw new ArgumentException("error: output file name not set.");
        }

        if (recordsAmount == null)
        {
            throw new ArgumentException("error: amount of records not set.");
        }

        if (startId == null)
        {
            throw new ArgumentException("error: start id not set.");
        }

        if (!outputType.Equals("csv", StringComparison.InvariantCultureIgnoreCase) &&
            !outputType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ArgumentException($"error: Invalid format type mode '{outputType}'.");
        }

        if (!outputFileName.EndsWith('.' + outputType.ToLower()))
        {
            throw new ArgumentException(
                $"error: Invalid format of output file '{outputFileName}. Should be '.{outputType.ToLower()}.");
        }

        const int minAmount = 1;
        if (!int.TryParse(recordsAmount, out int amount) || amount < minAmount)
        {
            throw new ArgumentException($"error: Invaild amount of records. Should be integer more than zero.");
        }

        const int minStartId = 1;
        if (!int.TryParse(startId, out int id) || id < minStartId)
        {
            throw new ArgumentException($"error: Invaild start id. Should be integer equal or more thasn 1.");
        }
    }
}