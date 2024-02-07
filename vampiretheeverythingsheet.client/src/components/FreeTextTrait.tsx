import { useId, useState, FocusEvent, ChangeEvent, ReactElement } from 'react';

export interface FreeTextTraitProps {
    name: string;
    initialValue?: string;
}

export function FreeTextTrait({ name, initialValue = "" }: FreeTextTraitProps): ReactElement {
    const textID = useId();
    const [ value, setValue ] = useState(initialValue);

    return (
        <div>
            <div>
                <label htmlFor={textID}>{name}: </label>
            </div>
            <div>
                <input type="text" id={textID} defaultValue={value}
                    onBlur={(event: FocusEvent<HTMLInputElement>) => { setValue(event.target.value || ""); event.preventDefault(); event.stopPropagation(); return false; }}
                    onChange={(event: ChangeEvent<HTMLInputElement>) => { setValue(event.target.value || ""); event.preventDefault(); event.stopPropagation(); return false; } }
                />
            </div>
        </div>
    );
}

export default FreeTextTrait;