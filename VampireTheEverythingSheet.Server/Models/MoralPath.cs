using System.Collections.ObjectModel;
using System.Data;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class MoralPath
    {
        //TODO: Do we need session memoization for these objects? Probably. Unless we want to simulate pulling from the DB every time?
        public static ReadOnlyDictionary<string, MoralPath> AllPaths { get; } = GetAllPaths();

        /// <summary>
        /// The name of this Path.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The Virtues of this Path, such as "Conscience and Self-Control".
        /// </summary>
        public string Virtues { get; private set; }

        /// <summary>
        /// The Bearing of this Path.
        /// </summary>
        public string Bearing { get; private set; }

        /// <summary>
        /// An integer representing the Resolve Penalty, if any, of this Path.
        /// This is always 0, -1, or -2.
        /// </summary>
        public int ResolvePenalty { get; private set; }

        private static ReadOnlyDictionary<string, MoralPath> GetAllPaths()
        {
            Dictionary<string, MoralPath> output = [];
            foreach(DataRow row in FakeDatabase.GetDatabase().GetPathData().Rows)
            {
                output[row["PATH_NAME"].ToString() ?? ""] = new MoralPath(row);
            }
            return new ReadOnlyDictionary<string, MoralPath>(output);
        }

        private MoralPath(DataRow row)
        {
            Name = row["PATH_NAME"].ToString() ?? "";
            Virtues = row["VIRTUES"].ToString() ?? "";
            Bearing = row["BEARING"].ToString() ?? "";
            ResolvePenalty = Utils.TryGetInt(row["RESOLVE_PENALTY"]) ?? 0;
        }
    }
}
