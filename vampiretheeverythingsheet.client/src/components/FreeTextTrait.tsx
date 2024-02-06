import { useId } from 'react';

export interface FreeTextTraitData {
    name: string;
    value: string;
}

export function FreeTextTrait(data: FreeTextTraitData) {
    //TODO: Update state on blur
    const textID = useId();
    return (
        <div>
            <label htmlFor={textID}>{data.name}: </label>
            <input type="text" id={textID} defaultValue={data.value} />
        </div>
    );
}

export default FreeTextTrait;