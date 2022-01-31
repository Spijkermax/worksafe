import React, { Component } from 'react'
import { Button, Form, Alert, FormGroup, FormLabel, FormControl, Row, Col, Card } from 'react-bootstrap'
import CardHeader from 'react-bootstrap/esm/CardHeader'

class AddEntry extends Component {
    constructor(props) {
        super(props)
        var entry = {
            Author: {
                Id: this.props.userId
            },
            Project: {
                Id: "xJcONkXPANC1452gfEho"
            },
            "Description": "",
            "Files": [
            ],
            "Impact": "",
            "Learning": "",
            "MindSet": "",
            "NextSteps": "",
            "Tags": [
            ]
        }
        this.state = {
            Entry: entry,
        }

        this.handleTitleChange = this.handleTitleChange.bind(this)
        this.handleDescriptionChange = this.handleDescriptionChange.bind(this)
        this.handleLearningChange = this.handleLearningChange.bind(this)
        this.handleMindsetChange = this.handleMindsetChange.bind(this)
        this.handleImpactChange = this.handleImpactChange.bind(this)
        this.handleNextStepsChange = this.handleNextStepsChange.bind(this)
        this.handleSubmit = this.handleSubmit.bind(this)
    }

    handleTitleChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.Title = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }

    handleDescriptionChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.Description = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }

    handleLearningChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.Learning = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }

    handleMindsetChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.MindSet = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }

    handleImpactChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.Impact = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }
    handleNextStepsChange(event) {
        this.setState(prevState => {
            let Entry = Object.assign({}, prevState.Entry)
            Entry.NextSteps = event.target.value
            return { Entry }
        })
        console.log(this.state)
    }

    async handleSubmit(event) {
        event.preventDefault()


        var entry = this.state.Entry;
        var options = {
            method: 'POST', // *GET, POST, PUT, DELETE, etc.
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(entry)
        }

        var url = "https://localhost:7001/api/users/" + this.state.Entry.Author.Id + "/entries"
        var response = await fetch(url, options)
        if (response.ok) {
            var result = await response.json()
            console.log(result)
            this.props.handleAddEntry(entry);
            this.props.handleShowAddEntry();
        } else {
            //show error
        }


    }

    render() {
        return (
            <Form>
                <Card>
                    <CardHeader>Add New Entry</CardHeader>
                    <Card.Body>
                        <Row sm={1} md={2}>
                            <Col>
                                <Form.Group>
                                    <FormLabel >Title</FormLabel>
                                    <FormControl type="text" value={this.state.Entry.Title} onChange={this.handleTitleChange} placeholder="Summarise the entry" />
                                </Form.Group>
                            </Col>

                            <Col md={3}>
                                <Form.Group>
                                    <FormLabel>Date</FormLabel>
                                    <FormControl type="date" value={this.state.Title} onChange={this.handleChange} name="date" />
                                </Form.Group>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Group className="mt-3">
                                <Form.Label>Description</Form.Label>
                                <Form.Control as="textarea" type="text" value={this.state.Entry.Description || ""} onChange={this.handleDescriptionChange} placeholder="Decribe the entry" />
                            </Form.Group>
                        </Row>
                        <Row>
                            <Col xs={5}>
                                <Form.Group className="mt-3">
                                    <Form.Label>Upload files</Form.Label>
                                    <Form.Control type="file" multiple />
                                </Form.Group>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Group className="mt-3">
                                <Form.Label>Learning</Form.Label>
                                <Form.Control as="textarea" value={this.state.Entry.Learning} onChange={this.handleLearningChange} placeholder="Decribe the learning" />
                            </Form.Group>
                        </Row>
                        <Row>
                            <Form.Group className="mt-3">
                                <Form.Label>Mindset</Form.Label>
                                <Form.Control type="text" value={this.state.Entry.Mindset} onChange={this.handleMindsetChange} placeholder="Mind set used" />
                            </Form.Group>
                        </Row>
                        <Row>
                            <Col>
                                <Form.Group className="mt-3">
                                    <Form.Label>Impact</Form.Label>
                                    <Form.Control as="textarea" value={this.state.Entry.Impact} onChange={this.handleImpactChange} placeholder="Impact of entry" />
                                </Form.Group>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Group className="mt-3">
                                <Form.Label>Next steps</Form.Label>
                                <Form.Control as="textarea" value={this.state.Entry.NextSteps} onChange={this.handleNextStepsChange} placeholder="Next steps based on this entry" />
                            </Form.Group>
                        </Row>
                    </Card.Body>

                    <Card.Footer>
                        <Button variant="success" onClick={this.handleSubmit}>
                            Add entry
                        </Button>
                    </Card.Footer>

                </Card>
            </Form>


        )
    }
}

export default AddEntry
