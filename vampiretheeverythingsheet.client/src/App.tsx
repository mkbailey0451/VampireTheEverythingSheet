import { useEffect, useState } from 'react';
import './App.css';
import { CharacterSheet, CharacterSheetProps } from './components/CharacterSheet';
import DropdownTrait from './components/GridElements/DropdownTrait';
import AutoGrid from './components/AutoGrid';
//TODO: Bootstrap?

var retrieveCharacterData: () => void;

function App() {

    const [character, setCharacter] = useState<CharacterSheetProps>();
    const [error, setError] = useState<boolean>(false);
    
    retrieveCharacterData = retrieveCharacterData || async function retrieveCharacterData() {
        const response: Response = await fetch('playerpersistence');
        const data = await response.json();

        if (response.ok) {
            setCharacter(data);
        }
        else {
            setError(true);
        }
    };

    useEffect(() => { retrieveCharacterData() }, []);

    if (error)
    {
        return <div>There was an issue retrieving the page. Please try again.</div>
    }

    if (typeof character === "undefined") {
        return <div>Retrieving data, please wait...</div>
    }

    return (
        <div className="gridContainer">
            <CharacterSheet {...character} />
        </div>
    );
}

export default App;