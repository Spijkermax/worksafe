import React, { Component } from 'react'
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';
import 'bootstrap/dist/js/bootstrap.js';
import 'bootstrap/dist/js/bootstrap.bundle.min';
import Moment from 'moment';
import Select from "react-dropdown-select";

class Dashboard extends Component {
    constructor(props) {
        super(props)
        this.state = {
            loading: true,
            entries: [],
            projects: []
        }
    }

    componentDidMount() {
        this.getProjects();
        this.getEntries();
    }

    async getProjects() {
        let result = await fetch("/api/projects");
        let data = await result.json();
        this.setState({ projects: data });
    }

    async getEntries() {
        let result = await fetch("/api/users/Unit Test User ID/entries");
        let data = await result.json();
        this.setState({ entries: data });
        this.setState({ loading: false });
    }

    render() {
        var projects = this.state.projects;
        class ProjectOption {
            constructor(label, value) {
                this.label = label;
                this.value = value;
            }
        }
        var projectOptions = [];
        projects.map(function (project, index) {
            const po = new ProjectOption(project.Title, project.Id);
            projectOptions[index] = po;
        });
        var entries = this.state.entries.map(function (entry, index) {
            return <a href="#" class="list-group-item list-group-item-action list-group-item-primary mb-2">
                <div class="d-flex w-100 justify-content-between">
                    <h4 class="mb-1">{entry.Description}</h4>
                </div>
                <p class="mb-1">{entry.Description}</p>
                <small>Last updated: {Moment(entry.TimeStamp).format('YYYY-MM-DD')} Owner:</small>
            </a>
        });

        if (this.state.loading) {
            return (
                <div>
                    <h2>Loading...</h2>
                </div>
            );
        }

        if (this.state.entries.length > 0) {
            return (
                <div>
                    <h2 class="mb-3">My Projects</h2>
                    <Select placeholder="All projects" options={projectOptions} onChange={(values) => this.setValues(values)} />
                    <div class="list-group">
                        <div class="d-flex">
                            <div class="mr-auto">
                                <h2>Feed</h2>
                            </div>
                            {/*<div class="mx-auto"></div>*/}
                            {/*<div class="p-2"><button type="button"*/}
                            {/*    class="btn btn-success">Get Report</button></div>*/}
                            {/*<div class="p-2"><button type="button" class="btn btn-primary">New Report</button>*/}
                            {/*</div>*/}
                        </div>
                        {entries}
                    </div>
                </div>

            );
        }

        return (
            <div>
                <h2>No Entries to Display...</h2>
            </div>
        );

    }
}

export default Dashboard;