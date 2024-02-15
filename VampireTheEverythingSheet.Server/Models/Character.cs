using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json.Serialization;
using VampireTheEverythingSheet.Server.DataAccessLayer;
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

        private HashSet<TemplateKey> _templateKeys = new HashSet<TemplateKey>();
        public void AddTemplate(TemplateKey key)
        {
            if(_templateKeys.Contains(key))
            {
                return;
            }
            if(_templateKeys.Count > 0 && key == TemplateKey.Mortal)
            {
                return;
            }
            _templateKeys.Add(key);

            Character template = Templates[key];

            if(template == null)
            {
                throw new ArgumentException("Unrecognized template " +  key + "in AddTemplate.");
            }

            foreach(Trait trait in template._traits.Values)
            {
                AddTrait(trait);
            }

            if(key != TemplateKey.Mortal)
            {
                _templateKeys.Remove(TemplateKey.Mortal);
            }
        }

        public void AddTrait(Trait trait)
        {
            if(_traits.Keys.Contains(trait.ID))
            {
                return;
            }
            _traits[trait.ID] = Trait.CreateTrait(this, trait);
        }

        public void RemoveTrait(Trait trait)
        {
            if (!_traits.Keys.Contains(trait.ID))
            {
                return;
            }
            _traits.Remove(trait.ID);
        }

        public void RemoveTemplate(TemplateKey keyToRemove)
        {
            if(!_templateKeys.Contains(keyToRemove))
            {
                return;
            }
            _templateKeys.Remove(keyToRemove);

            //build set of traits to keep
            HashSet<int> keepTraits = new HashSet<int>();

            foreach (TemplateKey templateKey in _templateKeys.Union([ TemplateKey.Mortal ]))
            {
                Character template = Templates[templateKey];
                if(template == null)
                {
                    continue;
                }
                keepTraits.UnionWith(template._traits.Keys);
            }

            //remove all others
            foreach(int traitID in _traits.Keys)
            {
                if(!keepTraits.Contains(traitID))
                {
                    _traits.Remove(traitID);
                }
            }
        }

        public string UniqueID { get; set; }

        private SortedDictionary<int,Trait> _traits = new SortedDictionary<int, Trait>();

        public IEnumerable<Trait> TopTextTraits
        {
            get
            {
                //There's two ways to do this - the LINQ way and the foreach-if way - and I'm not sure which I like better or which performs better.
                //I decided to mix it up for the demo project, to demonstrate that I can do both as much as anything else.
                foreach(Trait trait in _traits.Values)
                {
                    if(trait.Category == TraitCategory.TopText)
                    {
                        yield return trait;
                    }
                }
            }
        }

        public IEnumerable<Trait> PhysicalAttributes
        {
            get
            {
                //This is definitely more compact.
                return 
                    from trait in _traits.Values
                    where trait.Category == TraitCategory.Attribute
                        && trait.SubCategory == TraitSubCategory.Physical //If I'm reading the docs correctly, the underlying SortedDictionary eliminates the need for orderby.
                    select trait;
            }
        }

        public IEnumerable<Trait> SocialAttributes
        {
            get
            {
                //But I *think* this has less runtime overhead.
                //Might not make a real difference though. We might be talking performance gains on the order of microseconds,
                //which would only matter in very high-performance apps.
                //(But, you know, in that situation, we should absolutely do this - or an even faster method. I went even farther than this in IslandSanctuarySolver, after all.)
                foreach (Trait trait in _traits.Values)
                {
                    if (trait.Category == TraitCategory.Attribute && trait.SubCategory == TraitSubCategory.Social)
                    {
                        yield return trait;
                    }
                }
            }
        }

        public IEnumerable<Trait> MentalAttributes
        {
            get
            {
                //Going with this for now. It's easier to edit.
                return
                    from trait in _traits.Values
                    where trait.Category == TraitCategory.Attribute
                        && trait.SubCategory == TraitSubCategory.Mental
                    select trait;
            }
        }

        public IEnumerable<Trait> PhysicalSkills
        {
            get
            {
                return
                    from trait in _traits.Values
                    where trait.Category == TraitCategory.Skill
                        && trait.SubCategory == TraitSubCategory.Physical
                    select trait;
            }
        }

        public IEnumerable<Trait> SocialSkills
        {
            get
            {
                return
                    from trait in _traits.Values
                    where trait.Category == TraitCategory.Skill
                        && trait.SubCategory == TraitSubCategory.Social
                    select trait;
            }
        }

        public IEnumerable<Trait> MentalSkills
        {
            get
            {
                return
                    from trait in _traits.Values
                    where trait.Category == TraitCategory.Skill
                        && trait.SubCategory == TraitSubCategory.Mental
                    select trait;
            }
        }


        private static readonly ReadOnlyDictionary<TemplateKey, Character> Templates = new ReadOnlyDictionary<TemplateKey, Character>(GetAllTemplates());

        private static Dictionary<TemplateKey, Character> GetAllTemplates()
        {
            DataTable templateData = FakeDatabase.GetDatabase().GetTemplateData();

            //TODO
            throw new NotImplementedException();
        }
    }
}
