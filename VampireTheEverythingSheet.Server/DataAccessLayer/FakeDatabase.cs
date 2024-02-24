using Microsoft.Extensions.ObjectPool;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.DataAccessLayer
{
    /// <summary>
    /// Normally, an application of this kind would interface to a SQL backend via a class much like this one.
    /// However, this project is designed as a demonstration of the author's ability to develop code in React, and
    /// to be able to run on a local machine with no connection to a database for demo purposes. As such, this
    /// fake database layer with hardcoded values is provided. An interface has been created to allow for its easy replacement
    /// in the case that such is desirable.
    /// </summary>
    public class FakeDatabase : IDatabaseAccessLayer
    {
        #region Public members

        /// <summary>
        /// Returns the singleton instance of this database.
        /// </summary>
        /// <returns></returns>
        public static IDatabaseAccessLayer GetDatabase()
        {
            return _db;
        }

        public DataTable GetTraitData()
        {
            //Technically, we should do a deep copy here (and elsewhere in this class) to avoid mutability concerns, but this *is* a fake database anyway
            return _traits;
        }

        public DataTable GetCharacterTemplateData()
        {
            //TODO: Refactor because we're doing the trait template thing now
            //TODO: Revise this to reflect the DATA field below
            /*
             * This would normally be the result of a JOIN between the following tables:
             * TEMPLATES - would contain primary keys matching the Constants.TemplateKey enum, the name of the template, and probably nothing else
             * TEMPLATE_X_TRAIT - crosswalk table matching templates to their owned traits (primary key to primary key via foreign keys, with the TEMPLATE_X_TRAIT table having a PK of both keys)
             * TRAITS - table listing the ID, name, type (integer, free text, etc as enum values), category (top text, attribute, specific power, etc as enum values), subcategory (if applicable - 
             * things like physical/mental/social or Discipline/Lore/Tapestry - also as enum values), and data (whose meaning varies according to the other variables) of the trait.
             * 
             * The tables in this class are mainly meant to simulate that functionality.
             * */
            //TODO: Note archetypes and such above - probably from a function or stored proc
            DataTable output = new()
            {
                Columns =
                {
                    new DataColumn("TEMPLATE_ID", typeof(int)),
                    new DataColumn("TEMPLATE_NAME", typeof(string)),
                    new DataColumn("TRAIT_ID", typeof(int)),
                }
            };

            var rows =
                from templateRow in _templates.Rows.Cast<DataRow>()
                join templateXTraitRow in _template_x_trait.Rows.Cast<DataRow>()
                    on templateRow["TEMPLATE_ID"] equals templateXTraitRow["TEMPLATE_ID"]
                orderby templateRow["TEMPLATE_ID"], templateXTraitRow["TRAIT_ID"]
                select new object[]
                {
                    templateRow["TEMPLATE_ID"],
                    templateRow["TEMPLATE_NAME"],
                    templateXTraitRow["TRAIT_ID"],
                };

            foreach (object[] row in rows)
            {
                output.Rows.Add(row);
            }

            return output;
        }

        /// <summary>
        /// Returns a DataTable representing the moral Paths a character may follow.
        /// </summary>
        public DataTable GetPathData()
        {
            return _paths;
        }

        #endregion

        #region Private members

        /// <summary>
        /// The singleton instance of this database.
        /// </summary>
        private static readonly FakeDatabase _db = new();

        /// <summary>
        /// Since this database is a singleton, we naturally want its constructor to be private.
        /// </summary>
        private FakeDatabase() { }

        /// <summary>
        /// A Dictionary mapping a trait name to all trait IDs which have that name.
        /// In most cases, this is a one-to-one mapping, but some traits share a name 
        /// (such as the Generation Background and its derived top trait, or msgic paths belonging to multiple main Disciplines).
        /// </summary>
        private static readonly Dictionary<string, List<int>> _traitIDsByName = [];

        /// <summary>
        /// A DataTable emulating a crosswalk table mapping template IDs to the IDs of their respective traits.
        /// </summary>
        private static readonly DataTable _template_x_trait = BuildTemplateXTrait();
        private static DataTable BuildTemplateXTrait()
        {
            DataTable template_x_trait = new()
            {
                Columns =
                {
                    new DataColumn("TEMPLATE_ID", typeof(int)),
                    new DataColumn("TRAIT_ID", typeof(int)),
                }
            };

            #region Build template_x_trait
            string[] mortalTraitNames = [
                "Trait Max",
                "Magic Max",
                "Background Max",
                "Path Max",

                "Name",
                "Player",
                "Chronicle",
                "Nature",
                "Demeanor",
                "Concept",

                "Strength",
                "Dexterity",
                "Stamina",

                "Charisma",
                "Manipulation",
                "Composure",

                "Intelligence",
                "Wits",
                "Resolve",

                "Athletics",
                "Brawl",
                "Drive",
                "Firearms",
                "Larceny",
                "Stealth",
                "Survival",
                "Weaponry",

                "Animal Ken",
                "Empathy",
                "Expression",
                "Intimidation",
                "Persuasion",
                "Socialize",
                "Streetwise",
                "Subterfuge",

                "Academics",
                "Computer",
                "Crafts",
                "Investigation",
                "Medicine",
                "Occult",
                "Politics",
                "Science",

                "True Faith",

                "Allies",
                "Alternate Identity",
                "Contacts",
                "Fame",
                "Influence",
                "Mentor",
                "Resources",
                "Retainers",

                "Size",
                "Health",
                "Willpower",
                "Defense",
                "Speed",
                "Run Speed",
                "Initiative",
                "Soak",

                "Path"

                //TODO: Weapons, Physical Description
            ];

            foreach (string traitName in mortalTraitNames)
            {
                foreach (int id in _traitIDsByName[traitName])
                {
                    template_x_trait.Rows.Add((int)TemplateKey.Mortal, id);
                }
            }

            #endregion

            return template_x_trait;
        }

        /// <summary>
        /// A DataTable emulating a table of templates.
        /// </summary>
        private static readonly DataTable _templates = BuildTemplateTable();
        private static DataTable BuildTemplateTable()
        {
            DataTable templates = new()
            {
                Columns =
                {
                    new DataColumn("TEMPLATE_ID", typeof(int)),
                    new DataColumn("TEMPLATE_NAME", typeof(string)),
                }
            };

            foreach (TemplateKey key in Enum.GetValues(typeof(TemplateKey)))
            {
                templates.Rows.Add(new object[]
                {
                    (int)key,
                    key.ToString()
                });
            }

            return templates;
        }

        /// <summary>
        /// A DataTable emulating a table of traits.
        /// </summary>
        private static readonly DataTable _traits = BuildTraitTable();
        private static DataTable BuildTraitTable()
        {
            DataTable traits = new()
            {
                Columns =
                {
                    new DataColumn("TRAIT_ID", typeof(int)),
                    new DataColumn("TRAIT_NAME", typeof(string)),
                    new DataColumn("TRAIT_TYPE", typeof(string)),
                    new DataColumn("TRAIT_CATEGORY", typeof(int)),
                    new DataColumn("TRAIT_SUBCATEGORY", typeof(int)),
                    new DataColumn("TRAIT_DATA", typeof(string)),
                }
            };

            #region Build traits
            int traitID = 0;
            //this is not exactly the best way to do this, but again, this is a fake database and not really how we'd do any of this anyway
            object?[][] rawTraits =
            [
                #region Hidden Traits
                //TODO: Any?
                #endregion

                #region Top Traits
                [
                    traitID++,
                    "Name", //name
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
                ],
                [
                    traitID++,
                    "Player", //name
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
                ],
                [
                    traitID++,
                    "Chronicle", //name
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
                ],
                [
                    traitID++,
                    "Nature", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.PossibleValues}|" + string.Join("|", GetAllArchetypes())
                ],
                [
                    traitID++,
                    "Demeanor", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.PossibleValues}|" + string.Join("|", GetAllArchetypes())
                ],
                [
                    traitID++,
                    "Concept", //name
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
                ],
                [
                    traitID++,
                    "Clan", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.PossibleValues}|" + string.Join("|", GetAllClans())
                ],
                [
                    traitID++,
                    "Generation", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.IsVar}|GENERATION"
                ],
                [
                    traitID++,
                    "Sire", //name
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
                ],
                [
                    traitID++,
                    "Brood", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.IsVar}|BROOD\n{Keywords.PossibleValues}|" + string.Join("|", GetAllBroods())
                ],
                [
                    traitID++,
                    "Breed", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.DerivedOption}|[animal]|BROOD|" + string.Join("|", GetBroodBreedSwitch()) + "\n{Keywords.PossibleValues}|" + string.Join("|", GetAllBreeds())
                ],
                [
                    traitID++,
                    "Tribe", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.PossibleValues}|" + string.Join("|", GetAllTribes())
                ],
                [
                    traitID++,
                    "Auspice", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    $"{Keywords.PossibleValues}|" + string.Join("|", GetAllAuspices())
                ],
                //TODO more
                #endregion

                #region Attributes
                [
                    traitID++,
                    "Strength", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|1|TRAITMAX" //min, max
                ],
                [
                    traitID++,
                    "Dexterity", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                [
                    traitID++,
                    "Stamina", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],

                [
                    traitID++,
                    "Charisma", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                [
                    traitID++,
                    "Manipulation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                [
                    traitID++,
                    "Composure", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],

                [
                    traitID++,
                    "Intelligence", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                [
                    traitID++,
                    "Wits", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                [
                    traitID++,
                    "Resolve", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|1|TRAITMAX"
                ],
                #endregion

                #region Skills
                [
                    traitID++,
                    "Athletics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Brawl", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Drive", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Firearms", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Larceny", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Stealth", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Survival", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Weaponry", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],

                [
                    traitID++,
                    "Animal Ken", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Empathy", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Expression", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Intimidation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Persuasion", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Socialize", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Streetwise", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Subterfuge", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],

                [
                    traitID++,
                    "Academics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Computer", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Crafts", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Investigation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Medicine", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Occult", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Politics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                [
                    traitID++,
                    "Science", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    $"{Keywords.MinMax}|0|TRAITMAX"
                ],
                #endregion

                [
                    traitID++,
                    "True Faith", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Faith,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|5"
                ],

                //TODO: Physical Disciplines/etc need to have specific powers implemented a special way if we want to have all derived ratings - maybe don't and have a MINUSCOUNT rule or something?
                #region Disciplines
                [
                    traitID++,
                    "Animalism", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Auspex", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Celerity", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Chimerstry", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Dementation", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Dominate", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Fortitude", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Necromancy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.MainTraitMax}"
                ],
                [
                    traitID++,
                    "The Ash Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Bone Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Cenotaph Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Corpse in the Monster", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Grave’s Decay", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Path of the Four Humors", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Sepulchre Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "The Vitreous Path", //name
                    (int)TraitType.DerivedTrait, //TODO: Might beome IntegerTrait
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Necromancy"
                ],
                [
                    traitID++,
                    "Obeah", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Obfuscate", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Obtenebration", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Potence", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Presence", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Protean", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Quietus", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Serpentis", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Thaumaturgy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.MainTraitMax}"
                ],
                [
                    traitID++,
                    "Elemental Mastery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Green Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "Hands of Destruction", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "Movement of the Mind", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "Neptune’s Might", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Lure of Flames", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Blood", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Conjuring", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Corruption", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Mars", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Technomancy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of the Father’s Vengeance", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "Weather Control", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Thaumaturgy"
                ],
                [
                    traitID++,
                    "Vicissitude", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],

                //Branch Disciplines
                [
                    traitID++,
                    "Ogham", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Temporis", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Valeren", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],


                [
                    traitID++,
                    "Assamite Sorcery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.MainTraitMax}"
                ],
                [
                    traitID++,
                    "Awakening of the Steel", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:Awakening of the Steel"
                ],
                [
                    traitID++,
                    "Hands of Destruction", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:Hands of Destruction"
                ],
                [
                    traitID++,
                    "Movement of the Mind", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:Movement of the Mind"
                ],
                [
                    traitID++,
                    "The Lure of Flames", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:The Lure of Flames"
                ],
                [
                    traitID++,
                    "The Path of Blood", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:The Path of Blood"
                ],
                [
                    traitID++,
                    "The Path of Conjuring", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Assamite Sorcery\n{Keywords.MainTraitCount}|A:The Path of Conjuring"
                ],

                [
                    traitID++,
                    "Bardo", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Countermagic", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Daimoinon", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Flight", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Koldunic Sorcery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.MainTraitMax}"
                ],
                [
                    traitID++,
                    "The Way of Earth", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Koldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Fire", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Koldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Water", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Koldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Wind", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|MAGICMAX\n{Keywords.SubTrait}|Koldunic Sorcery"
                ],
                [
                    traitID++,
                    "Melpominee", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Mytherceria", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Sanguinus", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                [
                    traitID++,
                    "Visceratika", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.MainTraitCount}"
                ],
                #endregion

                //TODO: Lores, Tapestries, Knits, Arcana

                #region Specific Powers
                [
                    traitID++,
                    "Feral Whispers (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Beckoning (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Animal Succulence (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Quell the Beast (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Species Speech (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Subsume the Spirit (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Drawing Out the Beast (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shared Soul (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heart of the Pack (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Conquer the Beast (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Nourish the Savage Beast (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Subsume the Pack (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Taunt The Caged Beast (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heart of the Wild (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Unchain the Beast (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Animalism Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Animalism\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heightened Senses (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Aura Perception (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Spirit’s Touch (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ever-Watchful Eye (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Telepathy (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breach The Mind’s Sanctum (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mind to Mind (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Karmic Sight (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Through Another’s Eyes (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Into Another’s Heart (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Grave (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "False Slumber (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Auspex Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Auspex\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Enrich the Spirit (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Quell the Ravening Serpent (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Vows Unbroken (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gift of Apis (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Whisper of Dawn (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Boon of Anubis (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Bring Forth the Dawn (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Pillar of Osiris (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mummification (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ra’s Blessing (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Bardo\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Celerity Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Celerity\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ignis Fatuus (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fata Morgana (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Apparition (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Permanency (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Horrid Reality (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "False Resonance (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fatuus Mastery (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shared Nightmare (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Far Fatuus (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Suspension of Disbelief (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Figment (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Through The Cracks (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Chimerstry Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Chimerstry\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sense The Sin (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fear of the Void Below (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Conflagration (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Psychomachia (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Beastly Pact (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Concordance (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Herald of Topheth (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Contagion (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Call the Great Beast (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Passion (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fracture (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of Chaos (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Voice of Insanity (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Total Madness (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Babble (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sibyl’s Tongue (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weaving the Tapestry (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shattered Mirror (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Speak To The Stars (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Father’s Blood (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Daimoinon\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Command (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mesmerize (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Forgetful Mind (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Conditioning (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Obedience (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Possession (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Chain the Psyche (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Loyalty (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mass Manipulation (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Still the Mortal Flesh (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Far Mastery (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Speak Through the Blood (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dominate Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Dominate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Personal Armor (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Fortitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shared Strength (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Fortitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fortitude Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Fortitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Missing Voice (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Phantom Speaker (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Madrigal (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Virtuosa (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Siren’s Beckoning (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Persistent Echo (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shattering Crescendo (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Melpominee\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Riddle (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fae Sight (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Oath of Iron (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Walk In Dreaming (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fae Words (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Iron In The Mind (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Elysian Glade (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Geas (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Wyrd (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Mytherceria\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sense Vitality (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Anesthetic Touch (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Corpore Sano (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mens Sana (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Truce (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood Of My Blood (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flesh Of My Flesh (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Beating Heart (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Safe Passage (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Unburdening the Bestial Soul (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Lifesense (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Renewed Vigor (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Life Through Death (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Keeper Of The Flock (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Obeah Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obeah\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cloak of Shadows (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Unseen Presence (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mask of a Thousand Faces (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Vanish from the Mind’s Eye (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cloak the Gathering (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Conceal (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Soul Mask (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cache (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Veil of Blissful Ignorance (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Old Friend (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Avalonian Mist (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Create Name (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Obfuscate Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obfuscate\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shadow Play (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shroud of Night (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Arms of the Abyss (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Black Metamorphosis (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Tenebrous Form (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Tenebrous Mastery (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Darkness Within (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shadowstep (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shadow Twin (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Witness in Darkness (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Oubliette (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ahriman’s Demesne (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Keeper of the Shadowlands (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Obtenebration Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Obtenebration\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Consecrate the Grove (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Crimson Woad (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Inscribe the Curse (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Aspect of the Beast (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Moon and Sun (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Drink Dry the Earth (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Ogham\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earthshock (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Potence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flick (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Potence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Potence Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Potence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Awe (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dread Gaze (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Entrancement (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Summon (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Majesty (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Love (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Paralyzing Glance (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Spark of Rage (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cooperation (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ironclad Command (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Pulse of the City (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Presence Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Presence\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Beast (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Feral Claws (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earth Meld (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mist Form (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shape of the Beast (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Restore the Mortal Visage (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earth Control (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flesh of Marble (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shape of the Beast’s Wrath (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Spectral Body (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Purify the Impaled Beast (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Inward Focus (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Protean Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Protean\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Silence of Death (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Scorpion’s Touch (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dagon’s Call (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Baal’s Caress (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood Burn (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Taste of Death (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Purification (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ripples of the Heart (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Selective Silence (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Baal’s Bloody Talons (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Poison the Well of Life (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Songs of Distant Vitae (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Condemn the Sins of  the Father (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Quietus Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Quietus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Brother’s Blood (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Sanguinus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Octopod (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Sanguinus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gestalt (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Sanguinus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Walk of Caine (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Sanguinus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Coagulated Entity (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Sanguinus\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Eyes of the Serpent (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Tongue of the Asp (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Skin of the Adder (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Form of the Cobra (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Heart of Darkness (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cobra Fangs (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Divine Image (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heart Thief (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shadow of Apep (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Serpentis Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Serpentis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Hourglass of the Mind (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Recurring Contemplation (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Leaden Moment (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Patience of the Norns (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Clotho’s Gift (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Kiss of Lachesis (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "See Between Moments (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Clio’s Kiss (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cheat the Fates (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Temporis\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sense Infirmity (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Valeren\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Seek the Hated Foe (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Valeren\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Touch of Abaddon (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Valeren\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Armor of Caine’s Fury (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Valeren\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sword of Michael (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Valeren\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Malleable Visage (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fleshcraft (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Bonecraft (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Horrid Form (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Bloodform (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Chiropteran Marauder (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cocoon (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breath of the Dragon (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earth’s Vast Haven (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Zahhak (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Vicissitude Supremacy (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Vicissitude\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Stoneskin (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Claws of Stone (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Scry the Hearthstone (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Humble As The Earth (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Reshape the Fortress (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sand Form (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flesh to Stone (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Golem (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heightened Senses (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Aura Perception (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Spirit’s Touch (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ever-Watchful Eye (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Telepathy (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breach The Mind’s Sanctum (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mask of a Thousand Faces (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Vanish from the Mind’s Eye (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cloak the Gathering (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Beast (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earth Meld (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mist Form (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shape of the Beast (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Restore the Mortal Visage (••••• •)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Mind to Mind (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Conceal (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Soul Mask (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Earth Control (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flesh of Marble (••••• ••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Through Another’s Eyes (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cache (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Veil of Blissful Ignorance (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shape of the Beast’s Wrath (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Spectral Body (••••• •••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Into Another’s Heart (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Old Friend (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Purify the Impaled Beast (••••• ••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Grave (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "False Slumber (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Avalonian Mist (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Create Name (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Inward Focus (••••• •••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Visceratika\n{Keywords.PowerLevel}"
                ],


                //Assamite Sorcery
                [
                    traitID++,
                    "Confer with the Blade (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Awakening of the Steel\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Grasp of the Mountain (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Awakening of the Steel\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Pierce Steel’s Skin (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Awakening of the Steel\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Razor’s Shield (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Awakening of the Steel\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Spirit of Zulfiqar (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Awakening of the Steel\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Decay (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gnarl Wood (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Acidic Touch (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Atrophy (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Turn to Dust (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (1)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (2)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (3)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (4)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (5)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (1)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (2)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (3)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (4)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (5)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "A Taste for Blood (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood Rage (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood of Potency (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Theft of Vitae (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cauldron of Blood (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Summon the Simple Form (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Permanency (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Magic of the Smith (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Reverse Conjuration (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Power Over Life (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|A:The Path of Conjuring\n{Keywords.PowerLevel}"
                ],


                //Koldunic Sorcery
                [
                    traitID++,
                    "Grasping Soil (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Earth\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Endurance of Stone (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Earth\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Hungry Earth (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Earth\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Root of Vitality (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Earth\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Kupala’s Fury (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Earth\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fiery Courage (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Fire\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Combust (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Fire\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Wall of Magma (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Fire\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Heat Wave (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Fire\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Volcanic Blast (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Fire\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Pool of Lies (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Watery Haven (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fog Over Sea (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Minions of the Deep (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dessicate (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Doom Tide (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Water\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breath of Whispers (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Wind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Biting Gale (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Wind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breeze of Lethargy (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Wind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ride the Tempest (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Wind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Tempest (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Way of Wind\n{Keywords.PowerLevel}"
                ],


                //Necromancy
                [
                    traitID++,
                    "Shroudsight (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Ash Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Lifeless Tongues (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Ash Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dead Hand (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Ash Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ex Nihilo (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Ash Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shroud Mastery (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Ash Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Tremens (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Bone Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Apprentice’s Brooms (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Bone Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Shambling Hordes (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Bone Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Soul Stealing (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Bone Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Daemonic Possession (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Bone Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "A Touch of Death (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Cenotaph Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Reveal the Catene (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Cenotaph Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Tread Upon the Grave (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Cenotaph Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Death Knell (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Cenotaph Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Ephemeral Binding (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Cenotaph Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Masque of Death (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Corpse in the Monster\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cold of the Grave (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Corpse in the Monster\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Curse of Life (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Corpse in the Monster\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gift of the Corpse (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Corpse in the Monster\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gift of Life (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Corpse in the Monster\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Destroy the Husk (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Grave’s Decay\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Rigor Mortis (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Grave’s Decay\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Wither (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Grave’s Decay\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Corrupt the Undead Flesh (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Grave’s Decay\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dissolve the Flesh (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Grave’s Decay\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Whispers to the Soul (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Four Humors\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Kiss of the Dark Mother (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Four Humors\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dark Humors (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Four Humors\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Clutching the Shroud (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Four Humors\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Black Breath (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Four Humors\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Witness of Death (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Sepulchre Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Summon Soul (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Sepulchre Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Compel Soul (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Sepulchre Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Haunting (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Sepulchre Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Torment (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Sepulchre Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Dead (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Vitreous Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Aura of Decay (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Vitreous Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Soul Feast (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Vitreous Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Breath of Thanatos (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Vitreous Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Night Cry (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Vitreous Path\n{Keywords.PowerLevel}"
                ],


                //Thaumaturgy
                [
                    traitID++,
                    "Elemental Strength (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Elemental Mastery\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Wooden Tongues (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Elemental Mastery\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Animate the Unmoving (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Elemental Mastery\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Elemental Form (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Elemental Mastery\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Summon Elemental (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Elemental Mastery\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Herbal Wisdom (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Green Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Speed the Season’s Passing (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Green Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dance of Vines (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Green Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Verdant Haven (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Green Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Awaken the Forest Giants (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Green Path\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Decay (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Gnarl Wood (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Acidic Touch (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Atrophy (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Turn to Dust (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Hands of Destruction\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (1)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (2)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (3)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (4)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Movement of the Mind (5)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Movement of the Mind\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Eyes of the Sea (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Neptune’s Might\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Prison of Water (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Neptune’s Might\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood to Water (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Neptune’s Might\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Flowing Wall (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Neptune’s Might\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dehydrate (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Neptune’s Might\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (1)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (2)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (3)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (4)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Lure of Flames (5)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Lure of Flames\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "A Taste for Blood (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood Rage (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Blood of Potency (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Theft of Vitae (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Cauldron of Blood (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Blood\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Summon the Simple Form (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Permanency (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Magic of the Smith (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Reverse Conjuration (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Power Over Life (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Conjuring\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Contradict (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Corruption\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Subvert (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Corruption\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dissociate (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Corruption\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Addiction (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Corruption\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Dependence (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Corruption\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "War Cry (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Mars\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Strike True (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Mars\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Wind Dance (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Mars\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fearless Heart (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Mars\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Comrades at Arms (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Mars\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Analyze (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Technomancy\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Burnout (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Technomancy\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Encrypt/Decrypt (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Technomancy\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Remote Access (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Technomancy\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Telecommute (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of Technomancy\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Zillah’s Litany (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Father’s Vengeance\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "The Crone’s Pride (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Father’s Vengeance\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Feast of Ashes (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Father’s Vengeance\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Uriel’s Disfavor (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Father’s Vengeance\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Valediction (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|The Path of the Father’s Vengeance\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weather Control (1)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Weather Control\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weather Control (2)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Weather Control\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weather Control (3)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Weather Control\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weather Control (4)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Weather Control\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Weather Control (5)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|Weather Control\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Turning (•)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|True Faith\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Scourging (••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|True Faith\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Laying on Hands (•••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|True Faith\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Sanctification (••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|True Faith\n{Keywords.PowerLevel}"
                ],
                [
                    traitID++,
                    "Fear Not (•••••)", //name
                    (int)TraitType.SelectableTrait,
                    (int)TraitCategory.SpecificPower,
                    (int)TraitSubCategory.Discipline,
                    $"{Keywords.AutoHide}\n{Keywords.MinMax}|0|TRAITMAX\n{Keywords.SubTrait}|True Faith\n{Keywords.PowerLevel}"
                ],

                //TODO: More Specific Powers (oh golly...)
                #endregion

                #region Backgrounds
                //TODO: Put all in, sort alphabetically so trait order matches
                [
                    traitID++,
                    "Allies",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Alternate Identity",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Contacts",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Domain",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Fame",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Generation",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|GENERATIONMAX"
                ],
                [
                    traitID++,
                    "Herd",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Influence",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Mentor",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Resources",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Retainers",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Status",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|BACKGROUNDMAX"
                ],
                #endregion

                [
                    traitID++,
                    "Path",
                    (int)TraitType.PathTrait,
                    (int)TraitCategory.MoralPath,
                    (int)TraitSubCategory.None,
                    $"{Keywords.MinMax}|0|PATHMAX"
                ],
                //TODO: Create a PathInfo table with the data we need to populate the other fields (Bearing etc) on the front end and handle logic on the back end

                #region Vital Statistics

                [
                    traitID++,
                    "Size",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Speed",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Run Speed",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Health",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Willpower",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Defense",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Initiative",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Soak",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],
                [
                    traitID++,
                    "Blood Pool",
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.VitalStatistic,
                    (int)TraitSubCategory.None,
                    ""
                ],

                #endregion

                //TODO: Other Traits, Merits and Flaws, Weapons, Physical Description

            ];
            #endregion

            //template for the above:
            /*
                [
                    traitID++,
                    "",
                    (int)TraitType.FreeTextTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    ""
                ],
             */

            //build the actual datatable rows
            foreach (object?[] row in rawTraits)
            {
                traits.Rows.Add(row);
            }

            //build _traitIDsByName collection
            foreach (DataRow row in traits.Rows)
            {
                //we don't really have a way to log problems with this, but it should also never happen. Again, in a real project, we'd try/catch and log the error to a log file or the database,
                //but since we're essentially operating on dummy data, there's no real need.
                //Even so, I'm giving myself a spot to put a breakpoint in case something odd happens.
                if (row[1] == null || row[0] == null)
                {

                }
                string key = row[1].ToString() ?? "BAD TRAIT NAME";
                int id = int.Parse(row[0].ToString() ?? "-1");
                if (_traitIDsByName.TryGetValue(key, out List<int>? traitsOfThisName))
                {
                    traitsOfThisName.Add(id);
                }
                else
                {
                    _traitIDsByName[key] = [id];
                }
            }

            return traits;
        }

        private static readonly DataTable _paths = BuildPathTable();
        private static DataTable BuildPathTable()
        {
            DataTable pathData = new()
            {
                Columns =
                {
                    new DataColumn("PATH_NAME", typeof(string)),
                    new DataColumn("VIRTUES", typeof(string)),
                    new DataColumn("BEARING", typeof(string)),
                    new DataColumn("HIERARCHY_OF_SINS", typeof(int)),
                }
            };

            #region Build path data
            object[][] rawPathData =
            [
                [
                    "Humanity",
                    "Conscience and Self-Control",
                    "Humanity",
                    string.Join('\n',
                        "Selfish acts.",
                        "Injury to another (in Frenzy or otherwise, except in self-defense, etc).",
                        "Intentional injury to another (except self-defense, consensual, etc).",
                        "Theft.",
                        "Accidental violation (drinking a vessel dry out of starvation).",
                        "Intentional property damage.",
                        "Impassioned violation (manslaughter, killing a vessel in Frenzy).",
                        "Planned violation (outright murder, savored exsanguination).",
                        "Casual violation (thoughtless killing, feeding past satiation).",
                        "Gleeful or “creative” violation (let’s not go there)."
                    )
                ],
                [
                    "Assamia",
                    "Conviction and Self-Control",
                    "Resolve",
                    string.Join('\n',
                        "Feeding on a mortal without consent",
                        "Breaking a word of honor to a Clanmate",
                        "Refusing to offer a non-Assamite an opportunity to convert",
                        "Failing to take an opportunity to destroy an apostate from the Clan",
                        "Succumbing to frenzy",
                        "Wronging a mortal (such as by injury or theft), except by feeding",
                        "Killing a mortal in Frenzy, failing to take an opportunity to harm a wicked Kindred",
                        "Refusal to further the cause of Assamia, even when doing so is safe",
                        "Outright murder of a mortal",
                        "Acting against another Assamite, casual murder"
                    )
                ],
                [
                    "The Ophidian Path",
                    "Conviction and Self-Control",
                    "Devotion",
                    string.Join('\n',
                        "Pursuing one’s own indulgences instead of another’s",
                        "Refusing to aid another follower of the Path",
                        "Aiding a vampire in Golconda or anyone with True Faith",
                        "Failing to observe Apophidian religious ritual",
                        "Failing to undermine the current social order in favor of the Apophidians",
                        "Failing to do whatever is necessary to corrupt another",
                        "Failing to pursue arcane knowledge",
                        "Obstructing another Apophidian’s efforts, outright murder",
                        "Failing to take advantage of another’s weakness, casual killing",
                        "Refusing to aid in Set’s resurrection, gleeful killing"
                    )
                ],
                [
                    "The Path of the Archivist",
                    "Conviction and Self-Control",
                    "Sagacity",
                    string.Join('\n',
                        "Refusing to share knowledge with another",
                        "Refusing to pursue existing knowledge, going hungry",
                        "Refusing to research and expand the horizons of knowledge",
                        "Refusing to maintain a storehouse of knowledge",
                        "Acting with negligence in a library or other storehouse of knowledge",
                        "Burning a book (or destroying any other store of knowledge) or Frenzying for any reason other than to research something",
                        "Killing in a Frenzy, killing a knowledgeable person",
                        "Outright murder, killing a scholar or scientist",
                        "Casual violation, killing a fellow Archivist",
                        "Gleeful or creative violation, allowing knowledge to be permanently destroyed"
                    )
                ],
                [
                    "The Path of Bones",
                    "Conviction and Self-Control",
                    "Silence",
                    string.Join('\n',
                        "Showing a fear of death",
                        "Failing to study an occurrence of death",
                        "Causing the suffering of another for no personal gain",
                        "Postponing feeding when hungry",
                        "Succumbing to frenzy",
                        "Showing concern for another’s well-being",
                        "Accidental killing (such as in Frenzy), making a decision based on emotion rather than logic",
                        "Outright murder, inconveniencing oneself for another’s benefit",
                        "Casual murder, preventing a death for personal gain",
                        "Gleeful or creative violation, preventing a death for no personal gain"
                    )
                ],
                [
                    "The Path of Caine",
                    "Conviction and Instinct",
                    "Faith",
                    string.Join('\n',
                        "Failing to engage in research or study each night, regardless of circumstances",
                        "Failing to instruct other vampires in the Path of Caine",
                        "Befriending or co-existing with mortals",
                        "Showing disrespect to other students of Caine",
                        "Failing to ride the wave in frenzy",
                        "Succumbing to Rötschreck",
                        "Aiding a “humane” vampire, killing in a Frenzy",
                        "Failing to regularly test the limits of abilities and Disciplines, outright murder",
                        "Failing to pursue lore about vampirism when the opportunity arises, casual murder",
                        "Denying vampiric needs (by refusing to feed, showing compassion, or failing to learn about one’s vampiric abilities), gleeful or creative violation"
                    )
                ],
                [//TODO
                    "Humanity",
                    "Conscience and Self-Control",
                    "Humane",
                    string.Join('\n',
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                ],
                [
                    "Humanity",
                    "Conscience and Self-Control",
                    "Humane",
                    string.Join('\n',
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                ],
            ];
            #endregion

            foreach (object[] row in rawPathData)
            {
                pathData.Rows.Add(row);
            }

            return pathData;
        }

        #region Pseudo reference tables
        private static IEnumerable<string> GetAllArchetypes()
        {
            return [
                "Architect",
                "Autocrat",
                "Bon Vivant",
                "Bravo",
                "Capitalist",
                "Caregiver",
                "Celebrant",
                "Chameleon",
                "Child",
                "Competitor",
                "Conformist",
                "Conniver",
                "Creep Show",
                "Curmudgeon",
                "Dabbler",
                "Deviant",
                "Director",
                "Enigma",
                "Eye of the Storm",
                "Fanatic",
                "Gallant",
                "Guru",
                "Idealist",
                "Judge",
                "Loner",
                "Martyr",
                "Masochist",
                "Monster",
                "Pedagogue",
                "Penitent",
                "Perfectionist",
                "Rebel",
                "Rogue",
                "Sadist",
                "Scientist",
                "Soldier",
                "Survivor",
                "Thrill-Seeker",
                "Traditionalist",
                "Trickster",
                "Visionary"
            ];
        }

        private static IEnumerable<string> GetAllClans()
        {
            return [
                "Assamite (Hunter)",
                "Assamite (Vizier)",
                "Baali",
                "Blood Brothers",
                "Brujah",
                "Caitiff",
                "Cappadocian (Scholar)",
                "Cappadocian (Templar)",
                "Daughters of Cacophony",
                "Followers of Set",
                "Followers of Set (Warrior)",
                "Gangrel (City)",
                "Gangrel (Country)",
                "Gangrel (Pagan)",
                "Gargoyles",
                "Giovanni",
                "Harbingers of Skulls",
                "Kiasyd",
                "Lasombra",
                "Malkavian",
                "Nosferatu",
                "Ravnos",
                "Ravnos (Brahman)",
                "Salubri (Healer)",
                "Salubri (Warrior)",
                "Toreador",
                "Tremere",
                "True Brujah",
                "Tzimisce",
                "Ventrue"
            ];
        }

        private static IEnumerable<string> GetAllBroods()
        {
            return [
                "Kalebite",
                "Aragite",
                "Arumite",
                "Chatul (Tiger)",
                "Chatul (Lion)",
                "Chatul (Panther/Jaguar)",
                "Chatul (Cheetah)",
                "Chatul (Housecat)",
                "Chatul (Bobcat)",
                "Dabberite",
                "Devoroth",
                "Kohen",
                "Nychterid",
                "Tashlimite",
                "Tekton",
                "Tsayadite",
                "Zakarite",
                "Rishon",
                "Chargol"
            ];
        }

        private static IEnumerable<string> GetBroodBreedSwitch()
        {
            return [
                "Kalebite", "Lycanth",
                "Aragite", "Arachnes",
                "Arumite", "Alopex",
                "Chatul (Tiger)", "Ailouros",
                "Chatul (Lion)", "Ailouros",
                "Chatul (Panther/Jaguar)", "Ailouros",
                "Chatul (Cheetah)", "Ailouros",
                "Chatul (Housecat)", "Ailouros",
                "Chatul (Bobcat)", "Ailouros",
                "Dabberite", "Koraki",
                "Devoroth", "Melisses",
                "Kohen", "Arctos",
                "Nychterid", "Chiroptera",
                "Tashlimite", "Kouneli",
                "Tekton", "Kastoras",
                "Tsayadite", "Aetos",
                "Zakarite", "Loxodon",
                "Rishon", "[animal]",
                "Chargol", "Skathari",
            ];
        }

        private static IEnumerable<string> GetAllBreeds()
        {
            return [
                "Anthrope",
                "Yvrid",
                "[animal]"
            ];
        }

        private static IEnumerable<string> GetAllTribes()
        {
            return [
                "Ash Walkers",
                "Daughters of Zevah",
                "Firstborn",
                "Hell's Wardens",
                "Midnight Runners",
                "Shadows of Death",
                "Souls of Harmony",
                "Swords of Kaleb",
                "Undefeated",
                "Wolves of Tefar",
                "Blood Jackals",
                "Golden Moons",
                "Peerless Hunters",
                "Sanguine Kings",
                "Shadow-Hunters",
                "Velvet Whispers"
            ];
        }

        private static IEnumerable<string> GetAllAuspices()
        {
            return [
                "Scurra",
                "Logios",
                "Iudex",
                "Legatus",
                "Myrmidon",
            ];
        }
        #endregion

        #endregion
    }
}
