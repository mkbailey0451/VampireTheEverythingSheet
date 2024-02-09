import { ChangeEventHandler, ReactElement, useId } from 'react';
import { GridElement, GridElementProps } from './GridElement';

export interface DropdownTraitProps extends GridElementProps {
    dropdownTraitName: string;
    validValues: string[];
    initialValue?: string;
    changeCallback?: ChangeEventHandler;
}

export function DropdownTrait(props: DropdownTraitProps): ReactElement {
    const textID = useId();

    var { dropdownTraitName, validValues, initialValue = "", changeCallback = undefined } = props;

    return (
        <GridElement {...props } >
            <div>
                <label htmlFor={textID}>{dropdownTraitName}: </label>
            </div>
            <div>
                <select onChange={changeCallback} defaultValue={initialValue}>
                    <option value="" key=""></option>
                    {
                        validValues.map((value: string) => {
                            return <option value={value} key={value}>{value}</option>
                        })
                    }
                </select>
            </div>
        </GridElement>
    );
}

export default DropdownTrait;