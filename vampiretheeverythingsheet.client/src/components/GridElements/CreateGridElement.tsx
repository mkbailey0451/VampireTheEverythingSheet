import { ReactElement } from 'react';
import { IntegerTrait, IntegerTraitProps } from './IntegerTrait';
import { DropdownTrait, DropdownTraitProps } from './DropdownTrait';
import { GridElementProps } from './GridElement';
import { FreeTextTrait, FreeTextTraitProps } from './FreeTextTrait';

function CreateGridElement(props: GridElementProps): ReactElement {
    //I really hate that there's no easy way to narrow types in this language other than writing type-guarded member checking for every type
    //it's like we're just pretending to be a strongly-typed language
    //anyway, that's why we have all the name properties with different names on all the subtypes

    var intProps: any = props as IntegerTraitProps;
    if (intProps.integerTraitName) {
        return IntegerTrait(intProps);
    }

    var ddlProps: DropdownTraitProps = props as DropdownTraitProps;
    if (ddlProps.dropdownTraitName) {
        return DropdownTrait(ddlProps);
    }

    var freeTextProps = props as FreeTextTraitProps;
    if (freeTextProps.freeTextTraitName) {
        return FreeTextTrait(freeTextProps);
    }

    //TODO: Add more as they are implemented - mind the order, though
    return <div></div>;
}

export default CreateGridElement;