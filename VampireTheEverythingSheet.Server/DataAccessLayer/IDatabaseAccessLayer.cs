﻿using System.Data;
using System.Runtime.CompilerServices;
using VampireTheEverythingSheet.Server.Models;

namespace VampireTheEverythingSheet.Server.DataAccessLayer
{
    public interface IDatabaseAccessLayer
    {
        /// <summary>
        /// Get access to an instance of the access layer. The constructors for this subclass may be hidden to allow for singletons and load balancers.
        /// </summary>
        public abstract static IDatabaseAccessLayer GetDatabase();

        /// <summary>
        /// Returns a DataTable representing the TRAIT table in the database.
        /// </summary>
        public DataTable GetTraitData();

        /// <summary>
        /// Returns a DataTable representing the mapping of character templates to traits in the database.
        /// </summary>
        public DataTable GetCharacterTemplateData();

        public DataTable GetPathData();

    }
}
