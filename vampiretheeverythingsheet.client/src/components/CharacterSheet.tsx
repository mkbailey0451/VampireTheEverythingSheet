import { ReactElement } from 'react';

import { FreeTextTraitProps } from './GridElements/FreeTextTrait';
import { IntegerTraitProps } from './GridElements/IntegerTrait';
import { DropdownTraitProps } from './GridElements/DropdownTrait';
import AutoGrid from './AutoGrid';
import { PathProps } from './GridElements/Path';
import { MeritFlawProps } from './GridElements/MeritFlaw';
import { WeaponProps } from './GridElements/Weapon';

export interface CharacterSheetProps {
    
    TopTextTraits: (FreeTextTraitProps | DropdownTraitProps)[];

    PhysicalAttributes: IntegerTraitProps[];
    SocialAttributes: IntegerTraitProps[];
    MentalAttributes: IntegerTraitProps[];

    PhysicalSkills: IntegerTraitProps[];
    SocialSkills: IntegerTraitProps[];
    MentalSkills: IntegerTraitProps[];

    AddPowerTraits: IntegerTraitProps[];
    PowerTraits: IntegerTraitProps[];

    ValidSpecificPowers: Map<string, string[]>;
    SpecificPowers: Map<string, string[]>;

    AddBackgrounds: string[];
    Backgrounds: IntegerTraitProps[];

    ValidMeritsFlaws: MeritFlawProps[];
    MeritsFlaws: MeritFlawProps[];

    Path: PathProps;

    Beliefs: string[];

    ValidWeapons: string[];
    Weapons: WeaponProps[];

    PhysicalDescriptionBits: FreeTextTraitProps[];

    PhysicalDescription: string;

    History: string;

    Personality: string;

    Goals: string;

    BackgroundDetails: string[]; //TODO: Probably a Big Text Control of some description
}

//TODO: Use grid layout, passing current row between categories/grids/etc. and using the parent control as a master control for the whole thing. Hope like Hell it all actually works together.

export function CharacterSheet({ TopTextTraits }: CharacterSheetProps) {

    AddSplat = function AddSplat(splatType: string) {
        //TODO
    };
    RemoveSplat = function RemoveSplat(splatType: string) {
        //TODO
    };

    var row: number = 1;

    var elements: ReactElement[] = [];

    var rowCountCallback = (rowsConsumed: number) => { row += rowsConsumed; };

    elements.push(
        <AutoGrid traits={TopTextTraits} startingRow={row} rowCountCallback={ rowCountCallback } />
    );

    return (
        <div className="gridContainer" >
            { elements }
        </div>
    )
}

var AddSplat: (splatType: string) => void;
export { AddSplat };
var RemoveSplat: (splatType: string) => void;
export { RemoveSplat };

export default CharacterSheet;

export function DummyCharacterData(): CharacterSheetProps {

    var dummyData: CharacterSheetProps;

    dummyData = {
        TopTextTraits: [
            {
                freeTextTraitName: "Name",
                initialValue: "Big Z"
            },
            {
                freeTextTraitName: "Player",
                initialValue: "Chuck"
            },
            {
                freeTextTraitName: "Chronicle",
                initialValue: "The Muscleflex Chronicles"
            },
            {
                freeTextTraitName: "Nature",
                initialValue: "Wizard"
            },
            {
                dropdownTraitName: "Demeanor",
                initialValue: "Muscle",
                validValues: ["Muscle", "Wizard", "Muscle Wizard"]
            },
            {
                dropdownTraitName: "Concept",
                initialValue: "Muscle Wizard",
                validValues: ["Muscle", "Wizard", "Muscle Wizard"]
            },
            {
                freeTextTraitName: "Clan",
                initialValue: "Beefjah"
            },
            {
                freeTextTraitName: "Generation",
                initialValue: "Beefjah" //TODO: some kind of non-editable field thing for derived values
            },
            {
                freeTextTraitName: "Sire",
                initialValue: "Bob The Wizard"
            },
        ],

        PhysicalAttributes: [],
        SocialAttributes: [],
        MentalAttributes: [],

        PhysicalSkills: [],
        SocialSkills: [],
        MentalSkills: [],

        AddPowerTraits: [],
        PowerTraits: [],

        ValidSpecificPowers: new Map<string, string[]>,
        SpecificPowers: new Map<string, string[]>,

        AddBackgrounds: [],
        Backgrounds: [],

        ValidMeritsFlaws: [],
        MeritsFlaws: [],

        Path: {
            pathName: ""
        },

        Beliefs: [],

        ValidWeapons: [],
        Weapons: [],

        PhysicalDescriptionBits: [],

        PhysicalDescription: "",

        History: "",

        Personality: "",

        Goals: "",

        BackgroundDetails: []
    };



    return dummyData;
}
