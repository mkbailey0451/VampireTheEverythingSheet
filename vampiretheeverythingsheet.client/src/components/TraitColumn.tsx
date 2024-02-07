import { ReactElement } from 'react';

interface TraitColumnProps {
    children: any;
}

export function TraitColumn({ children }: TraitColumnProps): ReactElement {

    //TODO: consider removing this class, as it's kind of useless
    return (
        <div>
            { children }
        </div>
    );
}

export default TraitColumn;