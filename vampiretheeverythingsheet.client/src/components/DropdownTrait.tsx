import { ChangeEventHandler, ReactElement, useId } from 'react';

export interface DropdownTraitProps {
    name: string;
    validValues: string[];
    initialValue?: string;
    changeCallback?: ChangeEventHandler;
}

export function DropdownTrait({ name, validValues, initialValue = "" }: DropdownTraitProps) {
    const textID = useId();

    var options: ReactElement[] = [];

    //TODO: map values

    //TODO: Test to make sure the "selected" thing below works

    return (
        <div>
            <div>
                <label htmlFor={textID}>{name}: </label>
            </div>
            <div>
                <select>
                    <option value="" selected={initialValue === "" }></option>
                    { options }
                </select>
            </div>
        </div>
    );
}

export default DropdownTrait;