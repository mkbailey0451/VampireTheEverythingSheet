import { ReactElement } from 'react';

import { FreeTextTraitProps } from './GridElements/FreeTextTrait';
import { IntegerTraitProps } from './GridElements/IntegerTrait';
import { DropdownTraitProps } from './GridElements/DropdownTrait';
import AutoGrid from './AutoGrid';
import { PathProps } from './GridElements/Path';
import { MeritFlawProps } from './GridElements/MeritFlaw';
import { WeaponProps } from './GridElements/Weapon';

export interface CharacterSheetProps {
    
    topTextTraits: (FreeTextTraitProps | DropdownTraitProps)[];

    physicalAttributes: IntegerTraitProps[];
    socialAttributes: IntegerTraitProps[];
    mentalAttributes: IntegerTraitProps[];

    physicalSkills: IntegerTraitProps[];
    socialSkills: IntegerTraitProps[];
    mentalSkills: IntegerTraitProps[];

    addPowerTraits: string[];
    powerTraits: IntegerTraitProps[];

    validSpecificPowers: Map<string, string[]>;
    specificPowers: Map<string, string[]>;

    addBackgrounds: string[];
    backgrounds: IntegerTraitProps[];

    validMeritsFlaws: MeritFlawProps[];
    meritsFlaws: MeritFlawProps[];

    path: PathProps;

    beliefs: string[];

    validWeapons: string[];
    weapons: WeaponProps[];

    physicalDescriptionBits: FreeTextTraitProps[];

    physicalDescription: string;

    history: string;

    personality: string;

    goals: string;

    backgroundDetails: string[]; //TODO: Probably a Big Text Control of some description
}

//TODO: Use grid layout, passing current row between categories/grids/etc. and using the parent control as a master control for the whole thing. Hope like Hell it all actually works together.

export function CharacterSheet({ topTextTraits }: CharacterSheetProps) {

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
        <AutoGrid traits={topTextTraits} startingRow={row} rowCountCallback={ rowCountCallback } />
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
        topTextTraits: [
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

        physicalAttributes: [],
        socialAttributes: [],
        mentalAttributes: [],

        physicalSkills: [],
        socialSkills: [],
        mentalSkills: [],

        addPowerTraits: [],
        powerTraits: [],

        validSpecificPowers: new Map<string, string[]>,
        specificPowers: new Map<string, string[]>,

        addBackgrounds: [],
        backgrounds: [],

        validMeritsFlaws: [],
        meritsFlaws: [],

        path: {
            pathName: ""
        },

        beliefs: [],

        validWeapons: [],
        weapons: [],

        physicalDescriptionBits: [],

        physicalDescription: "",

        history: "",

        personality: "",

        goals: "",

        backgroundDetails: []
    };



    return dummyData;
}
