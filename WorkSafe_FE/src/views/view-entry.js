import React, { Component, useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPenToSquare } from "@fortawesome/free-solid-svg-icons";
import { Card, Button } from "react-bootstrap";
import "../styles/view-entry.css";
import Moment from 'react-moment';
import 'moment-timezone';

// Display view-entry card
class ViewEntry extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="view-entry mt-3">
                <Card>
                    <Card.Header style={{ backgroundColor: "#ffcc80" }} as="h4">
                        {this.props.entry.Title}
                        <div className="EditButton">
                            <Button variant="light" onClick={this.props.setEditing}><FontAwesomeIcon icon={faPenToSquare} /></Button>
                        </div>
                    </Card.Header>
                    <Card.Body>
                        <Card.Text> {this.props.entry.Description}</Card.Text>
                    </Card.Body>
                    <Card.Footer style={{ backgroundColor: "#fff3e0" }} as="h6">
                        Last Updated <Moment format="HH:MM, D MMM YYYY" withTitle>{this.props.entry.TimeStamp}</Moment> by {this.props.entry.Author.Name}
                    </Card.Footer>
                </Card>
            </div>
        );
    }
};

export default ViewEntry;
