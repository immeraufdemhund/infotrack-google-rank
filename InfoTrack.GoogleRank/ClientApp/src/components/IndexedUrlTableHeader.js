import React from "react";

export function IndexedUrlTableHeader({onClick, propertyName, text, sortProperty, sortDirection}) {
    let sortingClassName = "";
    if (propertyName === sortProperty) {
        if (sortDirection === 'up') {
            sortingClassName = "headerSortUp";
        }
        if (sortDirection === 'down') {
            sortingClassName = "headerSortDown";
        }
    }
    return (
        <th className={sortingClassName} onClick={() => onClick(propertyName)}>{text}</th>
    );
}
