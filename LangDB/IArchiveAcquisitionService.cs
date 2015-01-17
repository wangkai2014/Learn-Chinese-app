﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDB
{
    /// <summary>
    /// Interface for services to acquire archives and read them.
    /// </summary>
    public interface IArchiveAcquisitionService
    {
        /// <summary>
        /// Acquire a URI and return the lines within the file.
        /// </summary>
        /// <param name="uri">The URI of the archive to retrieve.</param>
        /// <returns>Each of the lines in the file.</returns>
        Task<IEnumerable<string>> GetLinesAsync(Uri uri);
    }
}