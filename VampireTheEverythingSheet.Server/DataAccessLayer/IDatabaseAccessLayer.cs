using System.Data;
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

        public DataTable GetTraitTemplateData();

        public DataTable GetCharacterTemplateData();

        public DataTable GetPathData();

    }
}
