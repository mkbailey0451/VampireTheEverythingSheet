import { useEffect, useState } from 'react';
import './App.css';
import { CharacterSheet, DummyCharacterData } from './components/CharacterSheet';
import DropdownTrait from './components/GridElements/DropdownTrait';
import AutoGrid from './components/AutoGrid';
//TODO: Bootstrap?


function App() {
    return (
        <div className="gridContainer">
            <CharacterSheet {...DummyCharacterData()} />
        </div>
    );
}

async function populateWeatherData() {
    const response = await fetch('weatherforecast');
    const data = await response.json();
    setForecasts(data);
}

export default App;