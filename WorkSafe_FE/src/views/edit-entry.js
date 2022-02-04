import React, { Component } from "react";
import { Select } from "react-dropdown-select";
import {
  Row,
  Col,
  Button,
  Form,
  FormLabel,
  FormControl,
  Card,
} from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faXmark } from "@fortawesome/free-solid-svg-icons";
import CardHeader from "react-bootstrap/esm/CardHeader";

class EditEntry extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Entry: this.props.entry,
    };

    this.handleTitleChange = this.handleTitleChange.bind(this);
    this.handleDescriptionChange = this.handleDescriptionChange.bind(this);
    this.handleLearningChange = this.handleLearningChange.bind(this);
    this.handleMindsetChange = this.handleMindsetChange.bind(this);
    this.handleImpactChange = this.handleImpactChange.bind(this);
    this.handleNextStepsChange = this.handleNextStepsChange.bind(this);
    this.handleProjectChange = this.handleProjectChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleTitleChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.Title = event.target.value;
      return { Entry };
    });
  }

  handleProjectChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      if (event[0] == undefined) {
        Entry.Project = {
          Project: {
            Id: "",
            Title: "",
          },
        };
      } else Entry.Project = this.getProject(event[0].value);
      console.log(Entry.Project);
      return { Entry };
    });
  }

  handleDescriptionChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.Description = event.target.value;
      return { Entry };
    });
  }

  handleLearningChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.Learning = event.target.value;
      return { Entry };
    });
  }

  handleMindsetChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.MindSet = event.target.value;
      return { Entry };
    });
  }

  handleImpactChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.Impact = event.target.value;
      return { Entry };
    });
  }
  handleNextStepsChange(event) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.NextSteps = event.target.value;
      return { Entry };
    });
  }

  handleSubmit = async (event) => {
    event.preventDefault();
    var options = {
      method: "PUT", // *GET, POST, PUT, DELETE, etc.
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(this.state.Entry),
    };

    var url =
      "https://localhost:7001/api/users/" +
      this.state.Entry.Author.Id +
      "/entries/" +
      this.props.entry.Id;
    var response = await fetch(url, options);

    var result = await response.json();
    if (response.ok) {
      this.props.handleUpdateEntry(this.state.Entry);
      this.props.setEditing();
    } else {
      ///some kind of error message
    }
  };

  getProject(id) {
    for (var i = 0; i < this.props.projects.length; i++) {
      if (this.props.projects[i].Id == id) {
        return this.props.projects[i];
      }
    }
    return null;
  }

  feedProjectsOptions() {
    var options = this.props.projects.map((project) => {
      return {
        value: project.Id,
        label: project.Title,
      };
    });
    console.log(options);
    return options;
  }

  handleTagChange(values) {
    this.setState((prevState) => {
      let Entry = Object.assign({}, prevState.Entry);
      Entry.Tags = values.map((tag) => tag.value);
      return { Entry };
    });
  }

  feedTagsOptions() {
    var options = this.props.tags.map((tag) => {
      return {
        label: tag,
        value: tag,
      };
    });
    return options;
  }

  entryTagsValue() {
    var value = this.props.entry.Tags.map((tag) => {
      return { label: tag, value: tag };
    });
    return value;
  }

  render() {
    const placeHolderOption = this.state.Entry.Project.Title;
    const projectsOptions = this.feedProjectsOptions();
    const tagsOptions = this.feedTagsOptions();
    const tagsValue = this.entryTagsValue();

    return (
      <Form>
        <Card>
          <CardHeader style={{ backgroundColor: "#D6E06D" }} as="h4">
            Update Entry
            <div className="editButton">
              <Button
                className="grow"
                variant="light"
                onClick={this.props.setEditing}
              >
                <FontAwesomeIcon icon={faXmark} />
              </Button>
            </div>
          </CardHeader>
          <Card.Body>
            <Row sm={1} md={2}>
              <Col>
                <Form.Group>
                  <FormLabel>Title</FormLabel>
                  <FormControl
                    type="text"
                    value={this.state.Entry.Title}
                    onChange={this.handleTitleChange}
                    placeholder="Summarise the entry"
                  />
                </Form.Group>
              </Col>

              <Col md={3}>
                <Form.Group>
                  <FormLabel>Date</FormLabel>
                  <FormControl
                    type="date"
                    value={this.state.Title}
                    onChange={this.handleChange}
                    name="date"
                  />
                </Form.Group>
              </Col>
            </Row>
            <Row sm={1} md={2}>
              <Form.Group className="mt-3">
                <Form.Label>Project</Form.Label>
                <Select
                  placeholder={placeHolderOption}
                  onChange={this.handleProjectChange}
                  options={projectsOptions}
                  backspaceDelete={false}
                  clearable={true}
                  dropdownHandle={false}
                />
              </Form.Group>
            </Row>
            <Row>
              <Form.Group className="mt-3">
                <Form.Label>Description</Form.Label>
                <Form.Control
                  as="textarea"
                  type="text"
                  value={this.state.Entry.Description || ""}
                  onChange={this.handleDescriptionChange}
                  placeholder="Decribe the entry"
                />
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
                <Form.Control
                  as="textarea"
                  value={this.state.Entry.Learning}
                  onChange={this.handleLearningChange}
                  placeholder="Decribe the learning"
                />
              </Form.Group>
            </Row>
            <Row>
              <Form.Group className="mt-3">
                <Form.Label>Mindset</Form.Label>
                <Form.Control
                  type="text"
                  value={this.state.Entry.Mindset}
                  onChange={this.handleMindsetChange}
                  placeholder="Mind set used"
                />
              </Form.Group>
            </Row>
            <Row>
              <Col>
                <Form.Group className="mt-3">
                  <Form.Label>Impact</Form.Label>
                  <Form.Control
                    as="textarea"
                    value={this.state.Entry.Impact}
                    onChange={this.handleImpactChange}
                    placeholder="Impact of entry"
                  />
                </Form.Group>
              </Col>
            </Row>
            <Row>
              <Form.Group className="mt-3">
                <Form.Label>Next steps</Form.Label>
                <Form.Control
                  as="textarea"
                  value={this.state.Entry.NextSteps}
                  onChange={this.handleNextStepsChange}
                  placeholder="Next steps based on this entry"
                />
              </Form.Group>
            </Row>
            <Row sm={1} md={2}>
              <Form.Group className="mt-3">
                <Form.Label>Tag</Form.Label>
                <Select
                  values={tagsValue}
                  options={tagsOptions}
                  onChange={(values) => this.handleTagChange(values)}
                  clearable={true}
                  dropdownHandle={false}
                  multi={true}
                  create={true}
                  color="#AAAAAA"
                />
              </Form.Group>
            </Row>
          </Card.Body>

          <Card.Footer>
            <Button variant="success" onClick={this.handleSubmit}>
              Save Changes
            </Button>
          </Card.Footer>
        </Card>
      </Form>
    );
  }
}

export default EditEntry;
