namespace VampireTheEverythingSheet.Server.Models
{
    public class IntegerTrait : Trait
    {
        public IntegerTrait(Character character, int minValue, int maxValue) : base(character)
        {
            MinValue = minValue;
            MaxValue = maxValue;
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
