using Microsoft.VisualBasic;
using System.Data;
using System.Globalization;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

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
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a copy of the supplied Trait but belonging to the supplied Character.
        /// </summary>
        public static Trait CreateTrait(Character character, Trait trait)
        {
            switch (trait)
            {
                case IntegerTrait intTrait: return new IntegerTrait(character, intTrait);
                //TODO all
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Parses the TRAIT_DATA field on a given row into more friendly tokens.
        /// </summary>
        private static IEnumerable<string[]> TokenizeData(DataRow row)
        {
            string? bigString = (row["TRAIT_DATA"] ?? "").ToString();
            if (string.IsNullOrEmpty(bigString))
            {
                yield break;
            }

            foreach(string bigToken in bigString.Split('\n'))
            {
                yield return bigToken.Split('|');
            }
        }

        /// <summary>
        /// Parses the TRAIT_DATA field on a given row into a temporary object that can be used to instantiate Trait subclass members.
        /// </summary>
        protected static TraitData GetTraitData(DataRow row)
        {
            //this is technically a case of unnecessary coupling of the abstract class and its subclasses,
            //but the alternative is basically rewriting this class in every subclass, and that's probably worse
            TraitData output = new TraitData();

            foreach (string[] tokens in TokenizeData(row))
            {
                switch(tokens[0])
                {
                    case Keywords.MinMax:
                        output.MinValue = tokens[1];
                        output.MaxValue = tokens[2];
                        break;
                    case Keywords.PossibleValues:
                        output.PossibleValues = tokens.Skip(1).ToArray(); //TODO: use
                        break;
                    case Keywords.AutoHide:
                        output.AutoHide = true;
                        break;
                    case Keywords.IsVar:
                        output.IsVar = tokens[1];
                        break;
                    case Keywords.DerivedOption:

                        for(int x = 1; x < tokens.Length - 1; x++)
                        {

                        }
                        break;
                }
            }

            return output;
        }

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
