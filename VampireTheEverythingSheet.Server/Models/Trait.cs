namespace VampireTheEverythingSheet.Server.Models
{
    public abstract class Trait
    {
        public Trait(Character character)
        {
            Character = character;
        }

        public abstract bool TryAssign(object newValue);

        public Character Character {  get; set; }
    }
}
