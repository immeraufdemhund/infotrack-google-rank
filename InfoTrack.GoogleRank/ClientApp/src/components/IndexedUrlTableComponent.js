import React, {useReducer} from "react";
import {Button, Table} from "reactstrap";
import './IndexedUrlTable.css';
import {IndexedUrlTableHeader} from "./IndexedUrlTableHeader";

export function IndexedUrlTableComponent({data, summary}) {
    const showMax = 1000;
    const showMin = 10;
    const columns = Object.freeze({
        index: Symbol("Index"),
        title: Symbol("Title"),
        url: Symbol("Url")
    });

    const reducer = (state, action) => {
        function compare(a, b, propertyName) {
            let aElement = a[propertyName];
            let bElement = b[propertyName];

            if (!aElement && !bElement) return 0;
            if (!aElement) return -1;
            if (!bElement) return 1;
            if (aElement < bElement) return -1;
            if (aElement > bElement) return 1;
            return 0;
        }

        switch (action.type) {
            case 'changeVisible':
                if (state.show === showMax) {
                    return {
                        ...state,
                        show: showMin, text: "show me moar!",
                        sortedData: data
                            .slice(0, showMin)
                            .sort((a, b) => compare(a, b, state.sortProperty))
                    }
                } else {
                    return {
                        ...state,
                        show: showMax, text: "show me less!",
                        sortedData: data
                            .sort((a, b) => compare(a, b, state.sortProperty))
                    }
                }
            case 'sort':
                if (!action.byProperty) throw new Error(`missing byProperty property in action when trying to sort`);
                const newDirection = state.sortDirection === 'up' ? 'down' : 'up';
                const newSort = newDirection === 'up' ?
                    (a, b) => compare(a, b, action.byProperty) :
                    (a, b) => compare(b, a, action.byProperty)

                return {
                    ...state,
                    sortProperty: action.byProperty,
                    sortDirection: newDirection,
                    sortedData: data
                        .slice(0, state.show)
                        .sort(newSort)
                };
            default:
                throw new Error(`missing action in state change in IndexedUrlTableComponent: '${action.type}'`);
        }
    };

    const initialState = {
        show: showMin,
        text: "show me moar!",
        sortProperty: columns.Index,
        sortDirection: 'up',
        sortedData: data
            .slice(0, showMin)
    };
    const [state, dispatch] = useReducer(reducer, initialState);
    return (
        <>
            <h2>{summary}</h2>
            <Button color="primary" onClick={() => dispatch({type: 'changeVisible'})}>{state.text}</Button>
            <Table size="sm" striped>
                <thead>
                <tr>
                    {Object.keys(columns)
                        .map((p, index) =>
                            <IndexedUrlTableHeader key={index}
                                                   onClick={() => dispatch({type: 'sort', byProperty: p})}
                                                   propertyName={p}
                                                   text={columns[p]}
                                                   sortProperty={state.sortProperty}
                                                   sortDirection={state.sortDirection}/>)
                    }
                </tr>
                </thead>
                <tbody>
                {state.sortedData.map((row, index) => {
                    return (
                        <tr key={index}>
                            {Object.keys(columns)
                                .map((p, index) =>
                                    <td>{row[p]}</td>)
                            }
                        </tr>
                    )
                })
                }
                </tbody>
            </Table>
        </>
    );
}
