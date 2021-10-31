import React, { Component } from 'react';
import { Redirect } from 'react-router';
import { HeistMembers } from './HeistMembers';
import { UpdateHeistSkills } from './UpdateHeistSkills';
import { EligibleMembers } from './EligibleMembers';
import { Heists } from './Heists';

export class HeistDetails extends Component {
    static displayName = HeistDetails.name;

    constructor(props) {
        super(props);
        this.state = {
            heist: {}, loading: true, redirect: false, redirectId: 0
        };
    }

    componentDidMount() {
        this.populateHeistData();
    }

    handleOnClick = (redirectId) => {
        // 1 - HeistMembers, 2 - UpdateSkills, 3 - EligibleMembers, 4 - Back to all heists
        this.setState({ redirect: true, redirectId: redirectId });
    }

    handleOnClickStart = async () => {
        const response = await fetch('heist/' + this.props.heistId + "/start", {
            method: 'PUT'
        });
        if (response.ok) {
            alert("Heist started!");
            this.state.heist.status = "IN_PROGRESS";
            this.setState({ heist: this.state.heist });
        }
        else {
            alert("Something went wrong!");
        }
    }
    
    static renderHeistDetails(heist, handleOnClick, handleOnClickStart) {
        return (
            <div>
                <h1>Heist</h1>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Location</th>
                            <th>Start</th>
                            <th>End</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            <tr>
                                <td>{heist.name}</td>
                                <td>{heist.location}</td>
                                <td>{heist.startTime.replace("T", " ").replace(":00.000Z", "")}</td>
                                <td>{heist.endTime.replace("T", " ").replace(":00.000Z", "")}</td>
                                <td>{heist.status}</td>
                                <td>
                                    {
                                        heist.status == "PLANNING" ?
                                        <button color="primary" className="px-4 btn btn-primary"
                                            onClick={() => handleOnClick(3)}
                                        >
                                            Eligible members
                                        </button> : null
                                    }

                                    <button className="btn btn-secondary ml-2" onClick={() => handleOnClick(1)}>Members</button>

                                    {
                                        heist.status == "READY" ? <button className="btn btn-success ml-2" onClick={() => handleOnClickStart()}>Start heist</button> : null
                                    }
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
                <br />
                <div className='d-flex justify-content-between mb-3'>
                    <h1 id="tabelLabel" >Skills</h1>
                    <button color="primary" className="px-4 btn btn-primary"
                        onClick={() => handleOnClick(2)}
                    >
                        Update skills
                    </button>
                </div>
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
                            heist.skills.length > 0 ?
                                heist.skills.map(skill =>
                                    <tr>
                                        <td>{skill.name}</td>
                                        <td>{skill.level}</td>
                                        <td>{skill.members}</td>
                                    </tr>
                                ) : <tr><td colSpan="5" className='text-center'>No skills currently</td></tr>
                        }
                    </tbody>
                </table>

                <button className="btn btn-primary mr-2" onClick={() => handleOnClick(4)}>Back to all heists</button>
            </div>
        );
    }

    render() {
        if (this.state.redirect) {
            if (this.state.redirectId == 1) {
                return (<HeistMembers heistId={this.props.heistId} />);
            }
            if (this.state.redirectId == 2) {
                return (<UpdateHeistSkills heistId={this.props.heistId} />);
            }
            if (this.state.redirectId == 3) {
                return (<EligibleMembers heistId={this.props.heistId} />);
            }
            if (this.state.redirectId == 4) {
                return (<Heists />);
            }
        }

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : HeistDetails.renderHeistDetails(this.state.heist, this.handleOnClick, this.handleOnClickStart)

        return (
            <div>
                {contents}
            </div>
        );
    }

    async populateHeistData() {
        const response = await fetch('heist/' + this.props.heistId);
        const data = await response.json();
        this.setState({ heist: data, loading: false });
    }
}
