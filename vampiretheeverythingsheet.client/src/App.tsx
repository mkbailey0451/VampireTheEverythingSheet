import { useEffect, useState } from 'react';
import './App.css';
import { IntegerTrait } from './components/IntegerTrait'
import { FreeTextTrait } from './components/FreeTextTrait'
import AutoGrid from './components/AutoGrid';
//TODO: Bootstrap?

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

function App() {
    return (
        <>
            <AutoGrid>
                <FreeTextTrait name="Name" />
                <FreeTextTrait name="Player" initialValue="Bob" />
                <FreeTextTrait name="Chronicle" />
                <FreeTextTrait name="Clan" />
                <FreeTextTrait name="Sect" />
                <FreeTextTrait name="Sire" />
                <FreeTextTrait name="Nature" />
                <FreeTextTrait name="Demeanor" />
            </AutoGrid>
            <AutoGrid>
                <IntegerTrait name="Strength" minValue={1} maxValue={5} />
                <IntegerTrait name="Dexterity" minValue={1} maxValue={5} />
                <IntegerTrait name="Stamina" minValue={1} maxValue={5} />
                <IntegerTrait name="Charisma" minValue={1} maxValue={5} />
                <IntegerTrait name="Manipulation" minValue={1} maxValue={5} />
                <IntegerTrait name="Composure" minValue={1} maxValue={5} />
                <IntegerTrait name="Intelligence" minValue={1} maxValue={5} />
                <IntegerTrait name="Wits" minValue={1} maxValue={5} />
            </AutoGrid>
        </>
    )
}

export default App;