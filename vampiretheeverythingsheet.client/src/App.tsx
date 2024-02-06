import { useEffect, useState } from 'react';
import './App.css';
import { IntegerTrait } from './components/IntegerTrait'
import { FreeTextTrait } from './components/FreeTextTrait'
//TODO: Bootstrap?

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

function App() {
    return (
        <div>
            <FreeTextTrait name="Name" value="Nerblob" />
            <IntegerTrait name={'Bobness'} value={3} minValue={1} maxValue={5} />
            <IntegerTrait name={'Billness'} value={4} minValue={0} maxValue={5} />
            <IntegerTrait name={'Catness'} value={0} minValue={0} maxValue={10} />
            <IntegerTrait name={'Dogness'} value={7} minValue={0} maxValue={10} />
        </div>
    )
}

export default App;