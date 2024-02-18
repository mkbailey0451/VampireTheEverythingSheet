using System.Collections.ObjectModel;
using System.Data;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class MoralPath
    {
        //TODO: Do we need session memoization for these objects? Probably.
        public static ReadOnlyDictionary<string, MoralPath> AllPaths { get; } = new ReadOnlyDictionary<string, MoralPath>(GetAllPaths());

        private static IDictionary<string, MoralPath> GetAllPaths()
        {
            Dictionary<string, MoralPath> output = new Dictionary<string, MoralPath>();
            foreach(DataRow row in FakeDatabase.GetDatabase().GetPathData().Rows)
            {
                output[row["PATH_NAME"].ToString() ?? ""] = new MoralPath(row);
            }
            return output;
        }

        private MoralPath(DataRow row)
        {
            //TODO
        }
    }
}
