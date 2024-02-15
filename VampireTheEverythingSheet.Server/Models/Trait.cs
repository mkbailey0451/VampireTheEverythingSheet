using System.Data;
using static VampireTheEverythingSheet.Server.DataAccessLayer.Constants;

namespace VampireTheEverythingSheet.Server.Models
{
    public abstract class Trait
    {
        /// <summary>
        /// Creates a Trait belonging to the supplied Character based on the data supplied by the DataRow.
        /// Expected fields are as follows: TRAIT_ID(int), TRAIT_NAME(string), TRAIT_TYPE(int valid for Constants.TraitType), 
        /// TRAIT_CATEGORY(int valid for Constants.TraitCategory), TRAIT_SUBCATEGORY(int valid for Constants.TraitSubCategory),
        /// DATA(specially formatted string)
        /// </summary>
        public static Trait CreateTrait(Character character, DataRow row)
        {
            switch ((TraitType)row["TRAIT_TYPE"])
            {
                case TraitType.IntegerTrait: return new IntegerTrait(character, row);
                    //TODO all
            }

            throw new ArgumentException("Unrecognized trait type in CreateTrait.");
        }

        public static Trait CreateTrait(Character character, Trait trait)
        {
            switch(trait)
            {
                case IntegerTrait intTrait: return new IntegerTrait(character, intTrait);
                    //TODO all
            }
            throw new ArgumentException("Unrecognized trait type in CreateTrait.");
        }
        //TODO: Subclasses might eliminate the need for the Type property - or not, considering we're sending it to the frontend

        /// <summary>
        /// Creates a copy of the supplied Trait but belonging to the supplied Character.
        /// </summary>
        protected Trait(Character character, Trait trait)
        {
            Character = character;
            ID = trait.ID;
            Name = trait.Name;
            Type = trait.Type;
            Category = trait.Category;
            SubCategory = trait.SubCategory;
            Data = trait.Data;
        }

        /// <summary>
        /// Creates a Trait belonging to the supplied Character based on the data supplied by the DataRow.
        /// Expected fields are as follows: TRAIT_ID(int), TRAIT_NAME(string), TRAIT_TYPE(int valid for Constants.TraitType), 
        /// TRAIT_CATEGORY(int valid for Constants.TraitCategory), TRAIT_SUBCATEGORY(int valid for Constants.TraitSubCategory),
        /// DATA(specially formatted string)
        /// </summary>
        protected Trait(Character character, DataRow row)
        {
            //there are a lot of possible exceptions here, but the correct thing to do in this case is throw them anyway
            Character = character;
            ID = (int)row["TRAIT_ID"];
            Name = (string)row["TRAIT_NAME"];
            Type = (TraitType)row["TRAIT_TYPE"];
            Category = (TraitCategory)row["TRAIT_CATEGORY"];
            SubCategory = (TraitSubCategory)row["TRAIT_SUBCATEGORY"];
            Data = (string)row["DATA"];
        }

        public abstract bool TryAssign(object newValue);

        public Character Character { get; private set; }

        public int ID { get; private set; }

        public string Name { get; private set; }

        public TraitType Type { get; private set; }

        public TraitCategory Category { get; private set; }

        public TraitSubCategory SubCategory { get; private set; }

        public string Data { get; private set; }
    }
}
