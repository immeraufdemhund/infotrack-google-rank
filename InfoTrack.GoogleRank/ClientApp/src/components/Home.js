import React, {useEffect, useState} from 'react';
import {IndexedUrlTableComponent} from "./IndexedUrlTableComponent";

export function Home(props) {
    const [state, setState] = useState({googleRankResult: [], loading: true});

    async function populateGoogleRank() {
        const response = await fetch('GoogleRank');
        return await response.json();
    }

    useEffect(() => {
        populateGoogleRank()
            .then((data) => {
                setState({googleRankResult: data, loading: false});
            });
    }, []);

    return (
        <div>
            <h1 id="tabelLabel">Google Results</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {state.loading
                ? <p><em>Loading...</em></p>
                : <IndexedUrlTableComponent googleRankResult={state.googleRankResult}/>}
        </div>
    );
}
