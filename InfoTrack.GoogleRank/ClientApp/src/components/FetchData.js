import React, {Component} from 'react';
import {IndexedUrlTableComponent} from "./IndexedUrlTableComponent";

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = {googleRankResult: [], loading: true};
    }

    componentDidMount() {
        this.populateGoogleRank();
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <IndexedUrlTableComponent googleRankResult={this.state.googleRankResult}/>;

        return (
            <div>
                <h1 id="tabelLabel">Google Results</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }

    async populateGoogleRank() {
        const response = await fetch('GoogleRank');
        const data = await response.json();
        this.setState({googleRankResult: data, loading: false});
    }
}
