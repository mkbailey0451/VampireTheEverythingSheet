namespace VampireTheEverythingSheet.Server.Models
{
    public class TraitData
    {
        /// <summary>
        /// Minimum numerical value of the Trait (if applicable).
        /// </summary>
        public string MinValue { get; set; } = "0";

        /// <summary>
        /// Maximum numerical value of the Trait (if applicable).
        /// </summary>
        public string MaxValue { get; set; } = "0";

        /// <summary>
        /// A list of all possible values of the Trait (for dropdown lists and the like).
        /// </summary>
        public string[] PossibleValues { get; set; } = [];

        /// <summary>
        /// If AutoHide is set, the Trait control will not appear on the frontend when its value is zero or empty.
        /// </summary>
        public bool AutoHide { get; set; } = false;

        /// <summary>
        /// If IsVar is not null, the Character will register a variable that tracks some value associated with this Trait.
        /// </summary>
        public string? IsVar { get; set; } = null;
        //TODO: Probably some flags for updating the Character from the Trait, vice versa, incrememting the Trait...

        /// <summary>
        /// Every key in this Dictionary is the "dummy" name of an option (stored in PossibleValues) whose actual value changes based on the 
        /// value of some variable stored on the Character. The associated value is the name of the variable. (For example, the Breed of an animal-form
        /// Kalebite is internally listed as "[animal]" in the database. This is associated with the BROOD variable, because the name of that breed varies
        /// according to the Brood of the Kalebite.)
        /// </summary>
        public Dictionary<string, string> DerivedOptionsLookup { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Every key in this Dictionary is the "dummy" name of an option (stored in PossibleValues) whose actual value changes based on the 
        /// value of some variable stored on the Character. The associated value is a Dictionary of values of that variable, mapped to the 
        /// correct value to transform that option to. (For example, the Breed of an animal-form Kalebite is internally listed as "[animal]" 
        /// in the database. This is associated with the BROOD variable, because the name of that breed varies according to the Brood of the Kalebite. 
        /// In this situation, the key "[animal]" would retrieve a Dictionary with keys of broods - Kalebite, Aragite, etc. - and values of their associated animal
        /// Breed names - Lycanth, Arachnes, etc.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> DerivedOptionsSwitch { get; set; } = new Dictionary<string, Dictionary<string, string>> ();
    }
}
