namespace VampireTheEverythingSheet.Server.DataAccessLayer
{
    public static class VtEConstants
    {
        /// <summary>
        /// This class contains a collection of database keywords used in the DATA field of the TRAITS table.
        /// These control the behavior of traits in various ways.
        /// </summary>
        public static class Keywords
        {
            /// <summary>
            /// A trait with the MinMax keyword has a minimum and maximum numeric value.
            /// Such Traits should always have numeric values (such as by having a TraitType
            /// of IntegerTrait).
            /// </summary>
            public const string MinMax = "MINMAX";

            /// <summary>
            /// The AutoHide keyword indicates that, if the Trait has a value of zero or the empty
            /// string, it should not appear on the character sheet. This is used for Discipline scores
            /// and similar.
            /// </summary>
            public const string AutoHide = "AUTOHIDE";

            /// <summary>
            /// The PossibleValues keyword indicates that the Trait has a list of possible, generally
            /// non-numeric, values.
            /// </summary>
            public const string PossibleValues = "VALUES";

            /// <summary>
            /// The IsVar keyword indicates that this Trait is a variable used elsewhere in the character
            /// sheet. Such values are generally numeric.
            /// </summary>
            public const string IsVar = "IS_VARIABLE";

            /// <summary>
            /// The DerivedOption keyword indicates that one of the possible values of this Trait has a name
            /// which varies depending on some character-wide variable.
            /// </summary>
            public const string DerivedOption = "DERIVED_OPTION";

            /// <summary>
            /// The MainTraitMax keyword indicates that this trait takes the maximum value of any subtrait belonging
            /// to it as its value.
            /// </summary>
            public const string MainTraitMax = "MAINTRAIT_MAX";

            /// <summary>
            /// The MainTraitCount keyword indicates that this trait takes the count of selected subtraits belonging
            /// to it as its value.
            /// </summary>
            public const string MainTraitCount = "MAINTRAIT_COUNT";

            /// <summary>
            /// The SubTrait keyword indicates that a given trait is a subtrait. A subtrait is a component trait of a 
            /// main trait, which in some way derives its value from its component subtraits. A trait may be both a 
            /// subtrait and a main trait, though circular references will result in undefined behavior and must not
            /// be created.
            /// </summary>
            public const string SubTrait = "SUBTRAIT";

            /// <summary>
            /// The PowerLevel keyword indicates that a selectable subtrait is of a certain level, and thus that its main 
            /// trait must be greater than or equal to its level minus one to select it.
            /// </summary>
            public const string PowerLevel = "POWER_LEVEL"; //TODO: Implement business rules
        }

        /// <summary>
        /// This enum defines the different character templates that a character may have, which then define what traits
        /// the character has or may select.
        /// </summary>
        public enum TemplateKey
        {
            Mortal = 0,
            Kindred = 1,
            Kalebite = 2,
            Fae = 3,
            Mage = 4,
        };

        /// <summary>
        /// This enum defines the types of different traits, which helps to control their appearance, behavior, and validation rules.
        /// </summary>
        public enum TraitType
        {
            /// <summary>
            /// A FreeTextTrait has a name and allows the user to associate any text with it.
            /// </summary>
            FreeTextTrait = 0,

            /// <summary>
            /// A DropdownTrait has a defined set of possible values and offers the user a dropdown list to select from these values.
            /// </summary>
            DropdownTrait = 1,

            /// <summary>
            /// An IntegerTrsit has a numeric value which the user can select by clicking on dots or through keyboard shortcuts.
            /// </summary>
            IntegerTrait = 2,

            /// <summary>
            /// A PathTrait reflects the character's moral Path, and allows the user to select both the Path and the rating thereof.
            /// The trait also displays certain information pertinent to the Path.
            /// </summary>
            PathTrait = 3, //TODO: Hierarchy of Sins?

            MeritFlawTrait = 4, //TODO: Is this the same as SelectableTrait, or do we need SpecificPowerTrait, or what?
            
            //TODO document more
            WeaponTrait = 5,
            DerivedTrait = 6,
            DerivedDropdownTrait = 7,
            SelectableTrait = 8, //TODO: Implement business rules
        };

        public enum TraitCategory
        {
            TopText = 0,
            Attribute = 1,
            Skill = 2,

            Progression = 3, //Rage, Gnosis, Arete, Wyrd

            Power = 4,
            SpecificPower = 5,
            Background = 6,
            VitalStatistic = 7,
            MeritFlaw = 8,
            Path = 9,
            Weapon = 10,
            PhysicalDescriptionBit = 11,
            Hidden = 12,
        };

        public enum TraitSubCategory
        {
            None = -1,

            Physical = 0,
            Social = 1,
            Mental = 2,

            Faith = 3,
            Discipline = 4,
            Lore = 5,
            Tapestry = 6,
            Knit = 7,
            Arcanum = 8,

            NaturalWeapon = 9,
            SelectableWeapon = 10,
        };

        public enum TraitValueDerivation
        {
            Standard,
            DerivedSum,
            DerivedOptions,
            MainTraitMax,
            MainTraitCount
        }
    }
}
