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

export function CharacterSheet(props: CharacterSheetProps) {

    AddSplat = function AddSplat(splatID: number) {
        //TODO
    };
    RemoveSplat = function RemoveSplat(splatID: number) {
        //TODO
    };

    var row: number = 1;

    var elements: ReactElement[] = [];

    var rowCountCallback = (rowsConsumed: number) => { row += rowsConsumed; };

    elements.push(
        <AutoGrid traits={props.TopTextTraits} startingRow={row} rowCountCallback={ rowCountCallback } />
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
