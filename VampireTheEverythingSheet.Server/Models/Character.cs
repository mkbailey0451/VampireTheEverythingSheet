using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;
using VampireTheEverythingSheet.Server.DataAccessLayer;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class Character
    {
        public Character(string uniqueID, params TemplateKey[] templates)
        {
            UniqueID = uniqueID;

            if(templates.Length == 0)
            {
                templates = [ TemplateKey.Mortal ];
            }

            foreach(TemplateKey template in templates)
            {
                AddTemplate(template);
            }

            //TODO
        }

        private readonly HashSet<TemplateKey> _templateKeys = [];
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

            CharacterTemplate template = CharacterTemplate.AllCharacterTemplates[key] ?? throw new ArgumentException("Unrecognized template " +  key + "in AddTemplate.");

            foreach (int traitID in template.TraitIDs)
            {
                AddTrait(traitID);
            }

            if(key != TemplateKey.Mortal)
            {
                _templateKeys.Remove(TemplateKey.Mortal);
            }
        }

        public void AddTrait(int traitID)
        {
            if(_traits.ContainsKey(traitID))
            {
                return;
            }
            _traits[traitID] = new Trait(this, TraitTemplate.AllTraitTemplates[traitID]);
        }

        public void RemoveTrait(int traitID)
        {
            if (!_traits.ContainsKey(traitID))
            {
                return;
            }
            //TODO: Handle removal of subtrait registrations, etc.
            _traits.Remove(traitID);
        }

        public void RemoveTemplate(TemplateKey keyToRemove)
        {
            if(!_templateKeys.Contains(keyToRemove))
            {
                return;
            }
            _templateKeys.Remove(keyToRemove);

            //build set of traits to keep
            HashSet<int> keepTraits = [];

            foreach (TemplateKey templateKey in _templateKeys.Union([ TemplateKey.Mortal ]))
            {
                if(CharacterTemplate.AllCharacterTemplates.TryGetValue(templateKey, out CharacterTemplate? template))
                {
                    keepTraits.UnionWith(template.TraitIDs);
                }
            }

            //remove all others
            foreach(int traitID in _traits.Keys)
            {
                if(!keepTraits.Contains(traitID))
                {
                    RemoveTrait(traitID);
                }
            }
        }

        /// <summary>
        /// A Dictionary mapping the names of variables to the unique IDs of Traits that represent these variables.
        /// This is done this way, rather than referencing the Trait directly, because it makes tracking their addition and removal easier.
        /// </summary>
        private readonly Dictionary<string, int> _variables = [];

        /// <summary>
        /// If the supplied string is numeric, returns an int representation of it.
        /// If the supplied string is the name of a variable registered to the character, returns the current value of that variable.
        /// Otherwise, returns null.
        /// This method will automatically convert retrieved numeric strings to integers when retrieving them.
        /// </summary>
        public object? GetVariable(string? variableName)
        {
            if(string.IsNullOrEmpty(variableName))
            {
                return null;
            }

            //Since we do not accept numeric strings as variable names, we provide this automatic parsing functionality as a courtesy to calling methods.
            //We could wait for the Utils.TryGetInt call below to handle this case, but that would result in a lot of unnecessary parsing on the way.
            if (int.TryParse(variableName, out int result))
            {
                return result;
            }

            int multiplier = 1;

            //support for negative variables - useful for sums
            if(variableName.StartsWith('-'))
            {
                multiplier = -1;
                variableName = variableName[1..];
            }

            if(!_variables.TryGetValue(variableName, out int traitID))
            {
                return null;
            }

            //we handle the variable expansion to the greatest extent possible, but not all variables are of a straightforward type
            object val = _traits[traitID].Value;

            if (Utils.TryGetInt(val, out int intVal))
            {
                return intVal * multiplier;
            }

            return val;
        }

        public bool GetVariable<T>(string variableName, out T? value)
        {
            object? val = GetVariable(variableName);
            if(val is T t)
            {
                value = t;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// A mapping of subtrait names to the sets of unique trait IDs corresponding to those traits.
        /// Much like with _variables, we do this to make it easier to track the addition and removal of such Traits.
        /// </summary>
        private readonly Dictionary<string, HashSet<int>> _subTraitRegistry = [];

        public int GetMaxSubTrait(string mainTrait)
        {
            if (_subTraitRegistry.TryGetValue(mainTrait, out var subTraits))
            {
                return
                (
                    from traitID in subTraits
                    select Utils.TryGetInt(_traits[traitID].Value) ?? int.MinValue
                ).Max();
            }
            return 0;
        }

        public int CountSubTraits(string mainTrait)
        {
            if(_subTraitRegistry.TryGetValue(mainTrait, out var subTraits))
            {
                return subTraits.Count;
            }
            return 0;
        }

        public void RegisterVariable(string variableName, Trait associatedTrait)
        {
            //we do not accept empty or numeric variable names
            if(variableName == "" || int.TryParse(variableName, out _))
            {
                return;
            }
            if(!_variables.ContainsKey(variableName))
            {
                _variables[variableName] = associatedTrait.UniqueID;
            }
            else
            {
                throw new Exception("Tried to register an already existing variable: " + variableName);
            }
        }

        public void RegisterSubTrait(string mainTrait, Trait subTrait)
        {
            if (_subTraitRegistry.TryGetValue(mainTrait, out var subTraits))
            {
                subTraits.Add(subTrait.UniqueID);
                return;
            }
            _subTraitRegistry[mainTrait] = [ subTrait.UniqueID ];
        }

        public string UniqueID { get; set; }

        private readonly SortedDictionary<int,Trait> _traits = [];

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
    }
}
