import React from 'react';

import { FreeTextTrait, FreeTextTraitProps } from './FreeTextTrait';
import { IntegerTrait, IntegerTraitProps } from './IntegerTrait';
import { DropdownTrait, DropdownTraitProps } from './DropdownTrait';

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

    addSpecificPowers: Map<string, string[]>;
    specificPowers: Map<string, string[]>;

    addBackgrounds: string[];
    backgrounds: IntegerTraitProps[];

    addMeritsFlaws: MeritFlawProps[];
    meritsFlaws: MeritFlawProps[];

    path: PathProps;

    beliefs: string[];

    addWeapons: string[];
    weapons: WeaponProps[];

    physicalDescriptionBits: FreeTextTraitProps[];

    physicalDescription: string;

    history: string;

    personality: string;

    goals: string;

    backgroyndDetails: BigTextTrait[]; //TODO: Probably a Big Text Control of some description
}

export function CharacterSheet() {
    AddSplat = function AddSplat(splatType: string) {
        //TODO
    };
    RemoveSplat = function RemoveSplat(splatType: string) {
        //TODO
    };
}

var AddSplat: (splatType: string) => void;
export { AddSplat };
var RemoveSplat: (splatType: string) => void;
export { RemoveSplat };

export default CharacterSheet;

function DummyCharacterData(): CharacterSheetProps {
    return {

    };
}
