using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.Utils.CommandHelper
{
    /// <summary>
    /// Parameters parser.
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// Parse parameters from select command.
        /// </summary>
        /// <param name="parameters"> Parameters. </param>
        /// <param name="actualSeparator"> Separator between properties.</param>
        /// <returns> IEnumerable of pairs property info and string representation of value . </returns>
        /// <exception cref="ArgumentException"> When using more than one operator type. </exception>
        public static IEnumerable<Tuple<PropertyInfo, string>> ParseSelectParameters(string parameters, out string actualSeparator)
        {
            const string andSeparator = " and ";
            const string orSeparator = " or ";

            actualSeparator = andSeparator;
            var splitedProperties = parameters.Split(andSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (splitedProperties.Length == 1)
            {
                splitedProperties = parameters.Split(orSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                actualSeparator = orSeparator;
            }
            else
            {
                var splitedProperitesByOr = parameters.Split(orSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (splitedProperitesByOr.Length != 1)
                {
                    throw new ArgumentException("You should use only one operator type: 'and' or 'or'");
                }
            }

            if (splitedProperties.Length == 1)
            {
                actualSeparator = andSeparator;
            }

            return ParseProperty(splitedProperties);
        }

        /// <summary>
        /// Parse parameters from update command.
        /// </summary>
        /// <param name="parameters"> Parameters. </param>
        /// <param name="separator"> Separator  between properties. </param>
        /// <returns> IEnumerable of pairs property info and string representation of value . </returns>
        public static IEnumerable<Tuple<PropertyInfo, string>> ParseUpdateParameters(string parameters, string separator)
        {
            var splitedProperties = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return ParseProperty(splitedProperties);
        }

        private static IEnumerable<Tuple<PropertyInfo, string>> ParseProperty(string[] properties)
        {
            List<Tuple<PropertyInfo, string>> propertiesTuple = new List<Tuple<PropertyInfo, string>>();
            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
            foreach (var property in properties)
            {
                var propertySplited = property.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (propertySplited.Length != 2)
                {
                    throw new ArgumentException($"Wrong updating property syntax : {property}");
                }

                const int propertyNameIndex = 0;
                const int propertyValueIndex = 1;

                if (!propertySplited[propertyValueIndex].StartsWith('\'') ||
                    !propertySplited[propertyValueIndex].EndsWith('\''))
                {
                    throw new ArgumentException("Property value should be in single quotes");
                }

                propertySplited[propertyValueIndex] = propertySplited[propertyValueIndex][1..^1];

                var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(propertySplited[propertyNameIndex], StringComparison.OrdinalIgnoreCase));

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"There is no such property as {propertySplited[propertyNameIndex]}");
                }

                propertiesTuple.Add(new Tuple<PropertyInfo, string>(propertyInfo, propertySplited[propertyValueIndex]));
            }

            return propertiesTuple;
        }
    }
}
