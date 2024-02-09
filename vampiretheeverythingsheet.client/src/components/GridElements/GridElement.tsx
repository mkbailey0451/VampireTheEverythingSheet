import { ReactElement } from 'react';

export interface GridElementProps {
    row?: number;
    col?: number;
    height?: number;
    width?: number;
}

//we don't want to require other controls that use GridEntryProps as a superclass to have to use the children prop
interface RealGridEntryProps extends GridElementProps {
    children: any;
}

export function GridElement(props: RealGridEntryProps): ReactElement {

    var { row = 1, col = 1, height = 1, width = 1 } = props;

    if (row < 1) {
        row = 1;
    }
    if (col < 1) {
        col = 1;
    }
    if (height < 1) {
        height = 1;
    }
    if (width < 1) {
        width = 1;
    }
    

    return (
        <div
            style={
                {
                    gridRowStart: row,
                    gridRowEnd: row + height,
                    gridColumnStart: col,
                    gridColumnEnd: col + width
                }
            }
        >
            {
                props.children
            }
        </div>
    );
}

export default GridElement;