using System.Data;
using static VampireTheEverythingSheet.Server.DataAccessLayer.Constants;

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
            //TODO: More if we need it
        }

        public IntegerTrait(Character character, DataRow row) : base(character, row)
        {
            //TODO
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

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public override bool TryAssign(object newValue)
        {
            if(!int.TryParse(newValue.ToString(), out int val))
            {
                return false;
            }

            if(val < MinValue || val > MaxValue)
            {
                return false;
            }

            _val = val;
            return true;
        }
    }
}
