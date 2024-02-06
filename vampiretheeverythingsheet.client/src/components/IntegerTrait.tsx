import { KeyboardEvent, MouseEvent, ReactNode } from 'react';
import { useId } from 'react';

//IntegerTrait renders a dot-based Trait, such as an Attribute or an Ability.

export interface IntegerTraitData {
    name: string;
    value: number;
    minValue: number;
    maxValue: number; //TODO: Not sure if this is for the front or back end
}


var DotClicked: (event: MouseEvent) => boolean;

//Renders a single dot in the Trait.
function Dot(dotIndex: number, filled: boolean) {
    
    return (
        <div onClick={DotClicked} data-index={dotIndex} data-filled={filled} key={dotIndex}>
            {
                filled
                    ? "\u2B24"
                    : "\u25EF"
            }
        </div>
    );
}

var KeyDownHandler: (event: KeyboardEvent) => boolean;

export function IntegerTrait(data: IntegerTraitData) {
    const valueID = useId();
    
    //we maintain a separate reference to this in the main class so we can easily handle updates
    var dots: ReactNode[] = [];
    
    //this needs to be in scope for the closure to access the data it needs, but we can avoid redefining it by memoizing its value
    DotClicked = DotClicked || function DotClicked(event: MouseEvent): boolean {
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
                if (dots.length > 1 && data.value > 1) {
                    desiredValue = 1;
                }
                //otherwise, unfill it
                else {
                    desiredValue = 0;
                }
            }
            else {
                //if there are filled dots past this one, unfill them but leave this one filled
                if (data.value > index + 1) {
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
        if (desiredValue < data.minValue) {
            desiredValue = data.minValue;
        }
        if (desiredValue > data.maxValue) {
            desiredValue = data.maxValue;
        }

        data.value = desiredValue;

        //TODO: update state - may be able to optimize by only rerendering dots

        return false;
    }

    //TODO: check if below comment is true
    //this needs to be in scope for the closure to access the data it needs, but we can avoid redefining it by memoizing its value
    KeyDownHandler = KeyDownHandler || function KeyDownHandler(event: KeyboardEvent): boolean {
        if (event.key === "+") {
            data.value++;
        }
        else if (event.key === "-") {
            data.value--;
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
        while (x < data.minValue) {
            dots.push(Dot(x, true));
            x++;
        }

        while (x < data.value) {
            dots.push(Dot(x, true));
            x++;
        }

        while (x < data.maxValue) {
            dots.push(Dot(x, false));
            x++;
        }

        return dots;
    }

    return (
        <div tabIndex={-1} onKeyDown={KeyDownHandler}>
            <label htmlFor={valueID}>{data.name}: </label>
            <div id={valueID}>
                <div className="HiddenText">Current value is {data.value}. Press the plus key to increase the value. Press the minus key to decrease the value.</div>
                <div role="presentation">
                    {GetDots()}
                </div>
            </div>
        </div>
    );
}

//TODO: keyboard handler, generic updater