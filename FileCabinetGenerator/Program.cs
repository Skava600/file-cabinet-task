using System;
using FileCabinetApp.Utils.Enums;

/// <summary>
/// The program class.
/// </summary>
public static class Program
{
    private const int ParameterNotSet = -1;

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
    }
}