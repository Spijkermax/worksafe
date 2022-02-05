import React, { Component, useState, useEffect } from "react";
import { Card, Button } from "react-bootstrap";
import "../styles/dashboard.css";
import { CardHeaderWithEditButton, CardFooter } from "../components"

// Display view-project card
class ViewProject extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="mt-3">
                <Card className="grow" onClick={this.props.setExpanded}>
                    <CardHeaderWithEditButton title={this.props.project.Title} subTitle={this.props.project.Owner.Name} setEditing={this.props.setEditing}/>
                     <Card.Body>
                        <Card.Text> {this.props.project.Description}</Card.Text>
                    </Card.Body>
                    <CardFooter timeStamp={this.props.project.TimeStamp} authorName={this.props.project.LastUpdatedBy.Name} />
                </Card>
            </div>
        );
    }
};

export default ViewProject;
