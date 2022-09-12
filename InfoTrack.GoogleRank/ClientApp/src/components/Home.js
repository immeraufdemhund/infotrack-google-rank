import React, {useEffect, useState} from 'react';
import {IndexedUrlTableComponent} from "./IndexedUrlTableComponent";

export function Home(props) {
    const [state, setState] = useState({googleRankResult: {data: [], summary: ''}, loading: true});

    async function populateGoogleRank() {
        const response = await fetch('GoogleRank');
        return await response.json();
    }

    useEffect(() => {
        populateGoogleRank()
            .then((response) => {
                setState({
                    googleRankResult: {
                        data: response.data.map(x => {
                            return {
                                index: x.index + 1,
                                url: x.url.replace("www.", ""),
                                title: x.title ?? 'Fake title',
                                clickableLink: x.fullUrl ?? 'https://google.com/search?num=100&q=something',
                                description: x.description ?? '<span class="r0bn4c rQMQod">Jan 29, 2021</span>By Comparison...'
                        }}),
                        summary: response.summary
                    },
                    loading: false
                });
            });
    }, []);

    return (
        <div>
            <h1 id="tableLabel">Google Results</h1>
            {state.loading
                ? <p><em>Loading...</em></p>
                : <IndexedUrlTableComponent data={state.googleRankResult.data} summary={state.googleRankResult.summary}/>}
        </div>
    );
}
