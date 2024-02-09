import { useId, useState, FocusEvent, ChangeEvent, ReactElement } from 'react';
import { GridElement, GridElementProps } from './GridElement';

export interface FreeTextTraitProps extends GridElementProps {
    freeTextTraitName: string;
    initialValue?: string;
}

export function FreeTextTrait(props: FreeTextTraitProps): ReactElement {
    var { freeTextTraitName, initialValue = "" } = props;
    const textID = useId();
    const [ value, setValue ] = useState(initialValue);

    console.log("ping");

    return (
        <GridElement {...props} >
            <div>
                <label htmlFor={textID}>{freeTextTraitName}: </label>
            </div>
            <div>
                <input type="text" id={textID} defaultValue={value}
                    onBlur={(event: FocusEvent<HTMLInputElement>) => { setValue(event.target.value || ""); event.preventDefault(); event.stopPropagation(); return false; }}
                    onChange={(event: ChangeEvent<HTMLInputElement>) => { setValue(event.target.value || ""); event.preventDefault(); event.stopPropagation(); return false; } }
                />
            </div>
        </GridElement>
    );
}

export default FreeTextTrait;