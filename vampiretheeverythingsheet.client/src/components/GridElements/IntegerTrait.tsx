import { KeyboardEvent, MouseEvent, ReactNode, ReactElement } from 'react';
import { useId, useState } from 'react';
import { GridElement, GridElementProps } from './GridElement';

export interface IntegerTraitProps extends GridElementProps {
    integerTraitName: string;
    initialValue?: number;
    minValue: number;
    maxValue: number;
}

//Renders a single dot in the Trait.
function Dot(dotIndex: number, filled: boolean, onDotClicked: (event: MouseEvent) => boolean): ReactElement {
    
    return (
        <span onClick={onDotClicked} data-index={dotIndex} data-filled={filled} key={dotIndex}>
            {
                filled
                    ? "\u2B24"
                    : "\u25EF"
            }
        </span>
    );
}

var KeyDownHandler: (event: KeyboardEvent) => boolean;

export function IntegerTrait(props: IntegerTraitProps): ReactElement {
    var { integerTraitName, initialValue, minValue, maxValue } = props;

    const valueID = useId();

    const [value, setValue] = useState(initialValue || minValue);
    
    //we maintain a separate reference to this in the main class so we can easily handle updates
    var dots: ReactNode[] = [];
    
    //this needs to be in scope for the closure to access the data it needs, but we can pass it to Dot later
    const DotClicked = function DotClicked(event: MouseEvent): boolean {
        event.preventDefault();
        event.stopPropagation();

        var index: number = parseInt(event.currentTarget.getAttribute("data-index") || "0", 10);
        var filled: boolean = event.currentTarget.getAttribute("data-filled") === "true";

        var desiredValue: number;

        //determine the user's desired dot value
        if (filled) {
            //if the final dot is filled and clicked, unfill it
            if (index === dots.length - 1) {
                desiredValue = index;
            }
            else if (index === 0) {
                //if the first dot is filled and clicked, and there are other filled dots, reduce value to 1
                if (dots.length > 1 && value > 1) {
                    desiredValue = 1;
                }
                //otherwise, unfill it
                else {
                    desiredValue = 0;
                }
            }
            else {
                //if there are filled dots past this one, unfill them but leave this one filled
                if (value > index + 1) {
                    desiredValue = index + 1;
                }
                //otherwise, unfill this one
                else {
                    desiredValue = index;
                }
            }
        }
        else {
            //the logic for unfilled dots is much simpler
            desiredValue = index + 1;
        }

        //bounds checks
        if (desiredValue < minValue) {
            desiredValue = minValue;
        }
        if (desiredValue > maxValue) {
            desiredValue = maxValue;
        }

        //console.log("Current value: " + value + "\nNew value: " + desiredValue);

        setValue(desiredValue);

        //TODO: may be able to optimize by only rerendering dots, but how? Does React do this already?

        return false;
    }

    //TODO: check if below comment is true
    //this needs to be in scope for the closure to access the data it needs, but we can avoid redefining it by memoizing its value
    KeyDownHandler = KeyDownHandler || function KeyDownHandler(event: KeyboardEvent): boolean {
        if (event.key === "+") {
            setValue(value + 1);
        }
        else if (event.key === "-") {
            setValue(value - 1);
        }
        else {
            return true;
        }

        event.preventDefault();
        event.stopPropagation();
        
        //TODO: update state - may be able to optimize by only rerendering dots

        return false;
    };

    function GetDots(): ReactNode[] {
        var x = 0;

        dots = [];

        //We could do bounds checking on this, but it's coming from the server so it's faster if we trust it
        while (x < minValue) {
            dots.push(Dot(x, true, DotClicked));
            x++;
        }

        while (x < value) {
            dots.push(Dot(x, true, DotClicked));
            x++;
        }

        while (x < maxValue) {
            dots.push(Dot(x, false, DotClicked));
            x++;
        }

        return dots;
    }

    return (
        <GridElement {...props} >
            <span tabIndex={-1} onKeyDown={KeyDownHandler}>
                <label htmlFor={valueID}>{integerTraitName}: </label>
                <span id={valueID}>
                    <div className="HiddenText">Current value is {value}. Press the plus key to increase the value. Press the minus key to decrease the value.</div>
                    <span role="presentation">
                        {GetDots()}
                    </span>
                </span>
            </span>
        </GridElement>
    );
}