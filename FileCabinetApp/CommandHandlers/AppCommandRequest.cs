using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class represents app request.
    /// </summary>
    internal class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command"> Request command. </param>
        /// <param name="parameters"> Request parameters. </param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets or sets request command.
        /// </summary>
        /// <value>
        /// request command.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets request parameters.
        /// </summary>
        /// <value>
        /// request parameters.
        /// </value>
        public string Parameters { get; set; }
    }
}
