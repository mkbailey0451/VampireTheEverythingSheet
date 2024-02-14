using System.Data;
using static VampireTheEverythingSheet.Server.DataAccessLayer.Constants;

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
        private static FakeDatabase _db = new FakeDatabase();
        public static IDatabaseAccessLayer GetDatabase()
        {
            return _db;
        }

        DataTable _templates;

        DataTable _traits;

        Dictionary<string, int> _traitNamesByID;

        private FakeDatabase()
        {
            _templates = new DataTable
            {
                Columns =
                {
                    new DataColumn("TEMPLATE_ID", typeof(int)),
                    new DataColumn("TEMPLATE_NAME", typeof(string)),
                }
            };

            foreach(TemplateKey key in Enum.GetValues(typeof(TemplateKey)))
            {
                _templates.Rows.Add(new object[]
                {
                    (int)key,
                    key.ToString()
                });
            }

            var traitID = 0;
            //this is not exactly the best way to do this, but again, this is a fake database and not really how we'd do any of this anyway
            object?[][] rawTraits =
            [
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
                    "VALUES\n" + string.Join("|",GetAllArchetypes())
                ],
                [
                    traitID++,
                    "Demeanor", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    "VALUES\n" + string.Join("|", GetAllArchetypes())
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
                    "VALUES\n" + string.Join("|", GetAllClans())
                ],
                [
                    traitID++,
                    "Generation", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None
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
                    "VALUES\n" + string.Join("|", GetAllBroods())
                ],
                [
                    traitID++,
                    "Breed", //name
                    (int)TraitType.DerivedDropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    "VALUES\n" + string.Join("|", GetAllBreeds())
                ],
                [
                    traitID++,
                    "Tribe", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    "VALUES\n" + string.Join("|", GetAllTribes())
                ],
                [
                    traitID++,
                    "Auspice", //name
                    (int)TraitType.DropdownTrait,
                    (int)TraitCategory.TopText,
                    (int)TraitSubCategory.None,
                    "VALUES\n" + string.Join("|", GetAllAuspices())
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
                    "1,TRAITMAX" //min, max
                ],
                [
                    traitID++,
                    "Dexterity", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Physical,
                    "1,TRAITMAX"
                ],
                [
                    traitID++,
                    "Stamina", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Physical,
                    "1,TRAITMAX"
                ],

                [
                    traitID++,
                    "Charisma", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    "1,TRAITMAX"
                ],
                [
                    traitID++,
                    "Manipulation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    "1,TRAITMAX"
                ],
                [
                    traitID++,
                    "Composure", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Social,
                    "1,TRAITMAX"
                ],

                [
                    traitID++,
                    "Intelligence", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    "1,TRAITMAX"
                ],
                [
                    traitID++,
                    "Wits", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    "1,TRAITMAX"
                ],
                [
                    traitID++,
                    "Resolve", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Attribute,
                    (int)TraitSubCategory.Mental,
                    "1,TRAITMAX"
                ],
                #endregion

                #region Skills
                [
                    traitID++,
                    "Athletics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1" //min, max
                ],
                [
                    traitID++,
                    "Brawl", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Drive", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Firearms", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Larceny", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Stealth", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Survival", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Weaponry", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Physical,
                    "0,-1"
                ],

                [
                    traitID++,
                    "Animal Ken", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Empathy", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Expression", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Intimidation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Persuasion", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Socialize", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Streetwise", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Subterfuge", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Social,
                    "0,-1"
                ],

                [
                    traitID++,
                    "Academics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Computer", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Crafts", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Investigation", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Medicine", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Occult", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Politics", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                [
                    traitID++,
                    "Science", //name
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Skill,
                    (int)TraitSubCategory.Mental,
                    "0,-1"
                ],
                #endregion

                [
                    traitID++,
                    "True Faith", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Faith,
                    "AUTOHIDE\nMINMAX\n0\n5"
                ],

                //TODO: Physical Disciplines need to have specific powers implemented a special way if we want to have all derived ratings
                #region Disciplines
                [
                    traitID++,
                    "Animalism", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Auspex", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Celerity", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Chimerstry", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Dementation", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Dominate", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Fortitude", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Necromancy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX"
                ],
                [
                    traitID++,
                    "The Ash Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Bone Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Cenotaph Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Corpse in the Monster", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Grave’s Decay", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Path of the Four Humors", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Sepulchre Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "The Vitreous Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nNecromancy"
                ],
                [
                    traitID++,
                    "Obeah", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Obfuscate", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Obtenebration", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Potence", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Presence", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Protean", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Quietus", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Serpentis", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Thaumaturgy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX"
                ],
                [
                    traitID++,
                    "Elemental Mastery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Green Path", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "Hands of Destruction", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "Movement of the Mind", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "Neptune’s Might", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Lure of Flames", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Blood", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Conjuring", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Corruption", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Mars", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of Technomancy", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "The Path of the Father’s Vengeance", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "Weather Control", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nThaumaturgy"
                ],
                [
                    traitID++,
                    "Vicissitude", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],

                //Branch Disciplines
                [
                    traitID++,
                    "Ogham", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Temporis", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Valeren", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],


                [
                    traitID++,
                    "Assamite Sorcery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                //TODO: Paths. Gonna need to make some of these apply to multiple somehow.
                [
                    traitID++,
                    "Bardo", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Countermagic", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Daimoinon", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Flight", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Koldunic Sorcery", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX"
                ],
                [
                    traitID++,
                    "The Way of Earth", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nKoldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Fire", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nKoldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Water", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nKoldunic Sorcery"
                ],
                [
                    traitID++,
                    "The Way of Wind", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nMAGICMAX\nSUBTRAIT\nKoldunic Sorcery"
                ],
                [
                    traitID++,
                    "Melpominee", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Mytherceria", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Sanguinus", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                [
                    traitID++,
                    "Visceratika", //name
                    (int)TraitType.DerivedTrait,
                    (int)TraitCategory.Power,
                    (int)TraitSubCategory.Discipline,
                    "AUTOHIDE\nMINMAX\n0\nTRAITMAX"
                ],
                #endregion

                //TODO: Lores, Tapestries, Knits, Arcana

                #region Backgrounds
                //TODO: Put all in, sort alphabetically so trait order matches
                //TODO: Advanced Backgrounds? If so, deal with MINMAX\n0\n5
                [
                    traitID++,
                    "Allies",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Alternate Identity",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Contacts",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Domain",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Fame",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Generation",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nGENERATIONMAX"
                ],
                [
                    traitID++,
                    "Herd",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Influence",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Mentor",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Resources",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Retainers",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                [
                    traitID++,
                    "Status",
                    (int)TraitType.IntegerTrait,
                    (int)TraitCategory.Background,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nBACKGROUNDMAX"
                ],
                #endregion

                [
                    traitID++,
                    "Path",
                    (int)TraitType.PathTrait,
                    (int)TraitCategory.Path,
                    (int)TraitSubCategory.None,
                    "MINMAX\n0\nPATHMAX\nVALUES\n" + string.Join("|", GetAllPaths())
                ],

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

                //TODO: Other Traits, Merits and Flaws, Weapons

            ];

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

            //TODO: _traits - columns and raw data
            _traits = new DataTable
            {
                Columns =
                {
                    new DataColumn("TRAIT_ID", typeof(string)),
                    new DataColumn("TRAIT_NAME", typeof(string)),
                    new DataColumn("TRAIT_CATEGORY", typeof(int)),
                    new DataColumn("TRAIT_SUBCATEGORY", typeof(int)),
                    new DataColumn("TRAIT_DATA", typeof(string)),
                }
            };

            foreach (object?[] row in rawTraits)
            {
                _traits.Rows.Add(row);
            }

            _traitNamesByID = new Dictionary<string, int>(_traits.Rows.Count);

            foreach(DataRow row in _traits.Rows)
            {
                //we don't really have a way to log problems with this, but it should also never happen. Again, in a real project, we'd try/catch and log the error to a log file or the database,
                //but since we're essentially operating on dummy data, there's no real need.
                //Even so, I'm giving myself a spot to put a breakpoint in case something odd happens.
                if(row[1] == null || row[0] == null || _traitNamesByID.ContainsKey(row[1].ToString() ?? "BAD TRAIT NAME"))
                {
                    
                }
                _traitNamesByID[row[1].ToString() ?? "BAD TRAIT NAME"] = int.Parse(row[0].ToString() ?? "-1");
            }
        }

        private IEnumerable<string> GetAllArchetypes()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllClans()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllBroods()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllBreeds()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllTribes()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllAuspices()
        {
            //TODO
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllPaths()
        {
            //TODO
            throw new NotImplementedException();
        }

        public DataTable GetTemplateData()
        {
            //TODO: Revise this to reflect the DATA field below
            /*
             * This would normally be the result of a fairly monstrous UNION/JOIN/mess between the following tables:
             * TEMPLATES - would contain primary keys matching the Constants.TemplateKey enum, the name of the template, and probably nothing else
             * TEMPLATE_X_TRAIT - crosswalk table matching templates to their owned traits (primary key to primary key via foreign keys, with the TEMPLATE_X_TRAIT table having a PK of both keys)
             * TRAITS - table listing the ID, name, type (integer, free text, etc as enum values), category (top text, attribute, specific power, etc as enum values), and 
             * subcategory (if applicable - things like physical/mental/social or Discipline/Lore/Tapestry - also as enum values) of the trait.
             * 
             * I'm not sure if it would be best to put every possible attribute of the different trait types on this table, or if it would be better to have supplemental
             * tables for that and key into them using the PK of TRAITS. If we take the second approach, this is where the UNION gets hairy, because we'd be doing a lot of LEFT JOIN nonsense to get all the 
             * data down, and even with the first approach, we end up with a lot of fields we're not using. It's not an ideal situation, but the alternative is to have a different table and crosswalk for every 
             * trait type (to associate them with templates), and I'm not sure that's any better.
             * 
             * We'd do it the mega-UNION way (regarldess of underlying table structure) to reduce the queries sent to the server, which almost always results in optimization. But it would be ideal
             * to test it both ways and make sure the mega-UNION was the fastest approach, because *woo golly* there are a lot of unused fields here.
             * */
            //TODO: Note archetypes and such above - probably from a function or stored proc
            return new DataTable
            {
                Columns =
                {
                    new DataColumn("TEMPLATE_ID", typeof(int)),
                    new DataColumn("TEMPLATE_NAME", typeof(string)),

                    new DataColumn("TRAIT_ID", typeof(string)),
                    new DataColumn("TRAIT_NAME", typeof(string)),
                    new DataColumn("TRAIT_TYPE", typeof(int)),
                    new DataColumn("TRAIT_CATEGORY", typeof(int)),
                    new DataColumn("TRAIT_SUBCATEGORY", typeof(string)),

                    //We don't actually send down default values from the DB when adding a template, so we don't send VALUE as a column here
                    //the values would be stored in a CHARACTER_X_TRAIT table somewhere
                    new DataColumn("DATA", typeof(string))
                    {
                        DefaultValue = ""
                    },
                }
            };
        }
    }
}
