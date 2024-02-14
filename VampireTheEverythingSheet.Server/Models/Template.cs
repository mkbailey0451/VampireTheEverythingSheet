using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.Constants;

namespace VampireTheEverythingSheet.Server.Models
{

    /// <summary>
    /// A Template represents a 
    /// </summary>
    public class Template
    {

        private Template()
        {

        }

        public static readonly ReadOnlyDictionary<TemplateKey, Template> Templates = new ReadOnlyDictionary<TemplateKey, Template>(GetAllTemplates());

        private static Dictionary<TemplateKey, Template> GetAllTemplates()
        {
            DataTable templateData = FakeDatabase.GetDatabase().GetTemplateData();

            //TODO
            throw new NotImplementedException();
        }
    }
}
