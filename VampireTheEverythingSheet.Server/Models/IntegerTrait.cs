using System.Data;
using System.Diagnostics;
using static VampireTheEverythingSheet.Server.DataAccessLayer.VtEConstants;

namespace VampireTheEverythingSheet.Server.Models
{
    public class IntegerTrait : Trait
    {
        /// <summary>
        /// Creates a copy of the supplied Trait but belonging to the supplied Character.
        /// </summary>
        public IntegerTrait(Character character, IntegerTrait trait) : base(character, trait)
        {
            MinValue = trait.MinValue;
            MaxValue = trait.MaxValue;
            AutoHide = trait.AutoHide;
            //TODO: More if we need it
        }

        public IntegerTrait(Character character, DataRow row) : base(character, row)
        {
            TraitData traitData = GetTraitData(row);
            MinValue = traitData.MinValue;
            MaxValue = traitData.MaxValue;
            AutoHide = traitData.AutoHide;
            //TODO: More if we need it
        }

        private int _val;
        public int Value
        {
            get
            {
                return _val;
            }
            private set
            {
                TryAssign(value);
            }
        }

        public string MinValue { get; set; }

        public string MaxValue { get; set; }

        public bool AutoHide { get; set; }

        public override bool TryAssign(object newValue)
        {
            if(!int.TryParse(newValue.ToString(), out int val))
            {
                return false;
            }

            //TODO min and max validation incl. vs. variables

            _val = val;
            return true;
        }
    }
}
