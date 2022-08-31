import React, {useState} from "react";
import {Button} from "reactstrap";

export function IndexedUrlTableComponent(props) {
    const showMax = 1000;
    const showMin = 10;
    const {googleRankResult} = props;
    const [state, setState] = useState({show: showMin, text: "show me moar!"});
    const showMore = () => {
        if (state.show === showMax) {
            setState({show: showMin, text: "show me moar!"});
        } else {
            setState({show: showMax, text: "show me less!"});
        }
    };
    return (
        <>
            <h2>{googleRankResult.summary}</h2>
            <Button color="primary" onClick={showMore}>{state.text}</Button>
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Index</th>
                    <th>Url</th>
                </tr>
                </thead>
                <tbody>
                {googleRankResult.data.filter(r => r.index < state.show).map(indexedUrl =>
                    <tr key={indexedUrl.index}>
                        <td>{indexedUrl.index + 1}</td>
                        <td>{indexedUrl.url}</td>
                    </tr>
                )}
                </tbody>
            </table>
        </>
    );
}
