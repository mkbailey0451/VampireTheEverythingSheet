import { ReactElement, ReactNode, Children } from 'react';
import { GridElementProps, GridElement } from './GridElements/GridElement';
import CreateGridElement from './GridElements/CreateGridElement';

//Describes a grid of traits in three columns, automatically formatted by column to minimize vertical space.
//For example, if 11 traits are given, the columns will have 4, 4, and 3 traits respectively.
//Uses the universal grid layout system described in CharacterSheet.tsx.

interface AutoGridProps {
    traits: GridElementProps[];
    columnCount?: number;
    startingRow?: number;
    rowCountCallback?: (rowsConsumed: number) => void;
}

export function AutoGrid({ traits, columnCount = 3, startingRow = 1, rowCountCallback = undefined }: AutoGridProps): ReactElement {

    var transformedElements: ReactElement[] = [];

    //determine how many items will be in each column, if we allow leftovers
    var quotient = Math.floor(traits.length / columnCount);
    //then, determine how many leftovers there are - these go in the earliest possible columns
    var modulus = traits.length % columnCount;

    //distribute children in column-major order
    var childIndex = 0;
    for (let x = 0; x < columnCount; x++) {

        //max is one higher for early columns that need leftovers
        let max = quotient +
            (
                x < modulus
                    ? 1
                    : 0
            );
                
        for (let y = 0; y < max; y++) {
            let trait = traits[childIndex];

            trait.row = startingRow + x;
            trait.col = y + 1;
            trait.width = 1;
            trait.height = 1;

            transformedElements.push(CreateGridElement(trait));

            childIndex++;
        }
    }

    if (rowCountCallback) {
        var rowsConsumed = modulus === 0
            ? quotient
            : quotient + 1;

        rowCountCallback(rowsConsumed);
    }
    
    return (
        <>
            {transformedElements}
        </>
    );
}

export default AutoGrid;