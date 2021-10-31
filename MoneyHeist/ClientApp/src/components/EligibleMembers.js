import React, { Component } from 'react';
import { Members } from './Members';
import { HeistDetails } from './HeistDetails';

export class EligibleMembers extends Component {
    static displayName = EligibleMembers.name;

    constructor(props) {
        super(props);
        this.state = {
            data: {}, loading: true, redirect: false, selected: []
        };
    }

    componentDidMount() {
        this.populateEligibleMembersData();
    }

    handleOnClick = () => {
        this.setState({ redirect: true });
    }

    handleOnDeleteClick = async (skillName) => {
        const response = await fetch('member/' + this.props.memberId + "/skills/" + skillName, { method: 'DELETE' });
        this.state.member.skills.splice(this.state.member.skills.findIndex(item => item.name === skillName), 1);
        this.setState({ member: this.state.member, loading: false });
    }

    handleOnClickSelect = (name) => {
        if (this.state.selected.includes(name)) {
            var index = this.state.selected.indexOf(name)
            this.state.selected.splice(index, 1);
        }
        else {
            this.state.selected.push(name);
        }
        this.setState({ selected: this.state.selected });
    }

    handleConfirmMembers = async () => {
        var requestBody = { members: this.state.selected };
        debugger;
        const response = await fetch('heist/' + this.props.heistId + "/members", {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody),
        });
        if (response.status == 204) {
            alert("Updated!");
        }
        else {
            alert("Something went wrong!");
        }
    }

    static renderEligibleMembersDetails(data, handleOnClick, handleConfirmMembers, handleOnClickSelect, selected) {
        return (
            <div>
                <h1>Heist Skills</h1>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Level</th>
                            <th>Members</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            data.skills.map(skill =>
                                <tr>
                                    <td>{skill.name}</td>
                                    <td>{skill.level}</td>
                                    <td>{skill.members}</td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
                <br />
                <div className='d-flex justify-content-between mb-3'>
                    <h1 id="tabelLabel" >Eligible members</h1>
                </div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Skills</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            data.members.length > 0 ?
                                data.members.map(member =>
                                    <tr>
                                        <td>{member.name}</td>
                                        <td>
                                            {
                                                member.skills.map(skill =>
                                                    <p>{skill.name}:  {skill.level}</p>
                                                )}
                                        </td>
                                        <td>
                                            <button style={{ backgroundColor: selected.includes(member.name) ? "darkgray" : "lightgray" }} className="btn btn-primary mr-2" onClick={() => handleOnClickSelect(member.name)}>Select</button>
                                        </td>
                                    </tr>
                                ) : <tr><td colSpan="5" className='text-center'>No members currently</td></tr>
                        }
                    </tbody>
                </table>
                <button className="btn btn-primary mr-2" onClick={() => handleOnClick()}>Heist Details</button>
                {
                    data.members.length > 0 ? <button className="btn btn-success mr-2" onClick={() => handleConfirmMembers()}>Confirm Members for Heist</button> : null
                }

            </div>
        );
    }

    render() {
        if (this.state.redirect) {
            return (<HeistDetails heistId={this.props.heistId} />);
        }

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : EligibleMembers.renderEligibleMembersDetails(this.state.data, this.handleOnClick, this.handleConfirmMembers, this.handleOnClickSelect, this.state.selected)

        return (
            <div>
                {contents}
            </div>
        );
    }

    async populateEligibleMembersData() {
        const response = await fetch('heist/' + this.props.heistId + "/eligible_members");
        const data = await response.json();
        this.setState({ data: data, loading: false });
    }
}
