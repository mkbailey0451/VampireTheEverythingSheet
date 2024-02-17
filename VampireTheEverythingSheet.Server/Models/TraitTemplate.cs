using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class TraitTemplate
    {
        public static ReadOnlyDictionary<int, TraitTemplate> AllTraitTemplates { get; } = GetAllTraitTemplates();

        private static ReadOnlyDictionary<int, TraitTemplate> GetAllTraitTemplates()
        {
            Dictionary<int, TraitTemplate> allTraits = [];
            foreach(DataRow row in FakeDatabase.GetDatabase().GetTraitTemplateData().Rows)
            {
                TraitTemplate template = new(row);
                allTraits[template.UniqueID] = template;
            }
            return new ReadOnlyDictionary<int, TraitTemplate>(allTraits);
        }

        private TraitTemplate(DataRow row)
        {
            UniqueID = (int)row["TRAIT_ID"];
            Name = (string)row["TRAIT_NAME"];
            Type = (TraitType)row["TRAIT_TYPE"];
            Category = (TraitCategory)row["TRAIT_CATEGORY"];
            SubCategory = (TraitSubCategory)row["TRAIT_SUBCATEGORY"];

            Data = (string)row["TRAIT_DATA"];
        }

        /// <summary>
        /// The trait ID of this Trait. Each different Trait has a unique ID.
        /// </summary>
        public int UniqueID { get; private set; }

        /// <summary>
        /// The name of the Trait, such as Strength, Path, or Generation. There are NOT guaranteed to be unique!
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of Trait, which determines its validation rules and how it is rendered on the front end.
        /// (This could have been implemented as subclasses, but the amount of duplicated code across different subclasses was becoming unreasonable.)
        /// </summary>
        public TraitType Type { get; private set; }

        /// <summary>
        /// The category of the Trait, which helps determine where on the page it will be rendered.
        /// </summary>
        public TraitCategory Category { get; private set; }

        /// <summary>
        /// The subcategory of the Trait, which helps determine where on the page it will be rendered and what character templates can use it.
        /// </summary>
        public TraitSubCategory SubCategory { get; private set; }

        /// <summary>
        /// The TRAIT_DATA field from the datyabase.
        /// It's easier to reprocess this every time (to ensure variables get properly registered and so on) than to try to sensibly copy all the data structures
        /// created by ProcessTraitData.
        /// </summary>
        public string Data { get; private set; }
    }
}
