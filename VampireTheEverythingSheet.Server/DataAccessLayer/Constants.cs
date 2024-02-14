namespace VampireTheEverythingSheet.Server.DataAccessLayer
{
    public static class Constants
    {
        public enum TemplateKey
        {
            Mortal = 0,
            Kindred = 1,
            Kalebite = 2,
            Fae = 3,
            Mage = 4,
        };

        public enum TraitType
        {
            FreeTextTrait = 0,
            DropdownTrait = 1,
            IntegerTrait = 2,
            PathTrait = 3,
            MeritFlawTrait = 4,
            WeaponTrait = 5,
            DerivedTrait = 6,
            DerivedDropdownTrait = 7,
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
    }
}
