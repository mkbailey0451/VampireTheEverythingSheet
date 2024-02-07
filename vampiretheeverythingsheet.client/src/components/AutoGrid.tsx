import { ReactElement, Children } from 'react';
import { TraitColumn } from "./TraitColumn";

//Describes a grid of traits in three columns, automatically formatted by column to minimize vertical space.
//For example, if 11 traits are given, the columns will have 4, 4, and 3 traits respectively.

interface AutoGridProps {
    children: any;
    columnCount?: number;
}

export function AutoGrid({ children, columnCount = 3 }: AutoGridProps): ReactElement {

    //This is normally considered bad practice because controls aren't exactly supposed to manipulate their child layout, but the entire point of this class is to do just that
    var childArray: any[] = Children.toArray(children);

    var cols: ReactElement[] = [];

    //determine how many items will be in each column, if we allow leftovers
    var quotient = Math.floor(childArray.length / columnCount);
    //then, determine how many leftovers there are - these go in the earliest possible columns
    var modulus = childArray.length % columnCount;

    //distribute children in column-major order
    var childIndex = 0;
    for (let x = 0; x < columnCount; x++) {
        //entries (or children) going in this column
        let columnEntries: ReactElement[] = [];

        //max is one higher for early columns that need leftovers
        let max = quotient +
            (
                x < modulus
                    ? 1
                    : 0
            );
                
        for (let y = 0; y < max; y++) {
            columnEntries.push(childArray[childIndex]);
            childIndex++;
        }

        cols.push(TraitColumn({ children: columnEntries }));
    }
    
    return (
        <div>
            {cols}
        </div>
    );
}

export default AutoGrid;