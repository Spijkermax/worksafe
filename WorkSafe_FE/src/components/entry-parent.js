import { EditEntry, ViewEntry, ViewFullEntry } from "../views";
import { Component } from "react";
//import EditEntry from "../Views/edit-entry";

class EntryParent extends Component {
    constructor(props) {
        super(props)
        this.state = {
            Editing: false,
            Expanded: false,
        }

        this.setEditing = this.setEditing.bind(this);
        this.setExpanded = this.setExpanded.bind(this);
    }

    setEditing() {
        if (this.state.Editing) {
            this.setState({
                Editing: false,
                Expanded: true,
            })
        }
        else {
            this.setState({
                Editing: true,
                Expanded: true,
            })
        }
    }

    setExpanded() {
        if (this.state.Expanded) {
            this.setState({ Expanded: false })
        }
        else {
            this.setState({ Expanded: true })
        }
    }

    render() {
        if (this.state.Editing) {
            return (
                <EditEntry entry={this.props.entry} setEditing={this.setEditing} handleUpdateEntry={this.props.handleUpdateEntry}/>
            )
        }
        if (this.state.Expanded) {
            return (
                <ViewFullEntry entry={this.props.entry} setExpanded={this.setExpanded} setEditing={this.setEditing} />
            )
        }

        return (
            <ViewEntry entry={this.props.entry} setExpanded={this.setExpanded} setEditing={this.setEditing} />
        )
    }



}


export default EntryParent;