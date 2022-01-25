using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler base with file cabinet service.
    /// </summary>
    internal class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        public ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.FileCabinetService = fileCabinetService;
        }

        /// <summary>
        /// Gets file cabinet service.
        /// </summary>
        /// <value>
        /// file cabinet service.
        /// </value>
        protected IFileCabinetService FileCabinetService { get; private set; }
    }
}
