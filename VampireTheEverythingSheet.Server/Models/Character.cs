using static VampireTheEverythingSheet.Server.DataAccessLayer.Constants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class Character
    {
        public Character(string uniqueID, IEnumerable<TemplateKey>? templates = null)
        {
            UniqueID = uniqueID;

            if(templates == null || !templates.Any())
            {
                templates = new List<TemplateKey> { TemplateKey.Mortal };
            }

            foreach(TemplateKey template in templates)
            {
                AddTemplate(template);
            }

            //TODO
        }

        public void AddTemplate(TemplateKey key)
        {
            //TODO
        }

        public void RemoveTemplate(TemplateKey key)
        {
            //TODO
        }

        public string UniqueID { get; set; }



        public IEnumerable<Trait> TopTextTraits
        {
            get;
        }

        public IEnumerable<Trait> PhysicalAttributes { get; set; }


    }
}
