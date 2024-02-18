using System.Collections.ObjectModel;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;
using System.Data;
using VampireTheEverythingSheet.Server.DataAccessLayer;

namespace VampireTheEverythingSheet.Server.Models
{
    public class CharacterTemplate
    {
        public static ReadOnlyDictionary<TemplateKey, CharacterTemplate> AllCharacterTemplates { get; } = new ReadOnlyDictionary<TemplateKey, CharacterTemplate>(GetAllCharacterTemplates());

        private static Dictionary<TemplateKey, CharacterTemplate> GetAllCharacterTemplates()
        {
            Dictionary<TemplateKey, CharacterTemplate> output = [];

            foreach (DataRow row in FakeDatabase.GetDatabase().GetCharacterTemplateData().Rows)
            {
                TemplateKey templateKey = (TemplateKey)row["TEMPLATE_ID"];

                if (!output.TryGetValue(templateKey, out CharacterTemplate? template))
                {
                    template = new CharacterTemplate(templateKey, (string)row["TEMPLATE_NAME"]);
                    output.Add(templateKey, template);
                }

                template._traitIDs.Add((int)row["TRAIT_ID"]);
            }

            return output;
        }

        private CharacterTemplate(TemplateKey uniqueID, string name)
        {
            UniqueID = uniqueID;
            Name = name;
            _traitIDs = [];
        }

        public TemplateKey UniqueID { get; set; }

        public string Name { get; private set; }

        private readonly HashSet<int> _traitIDs = [];
        public IEnumerable<int> TraitIDs
        {
            get
            {
                foreach(int id in _traitIDs)
                {
                    yield return id;
                }
            }
        }
    }
}
