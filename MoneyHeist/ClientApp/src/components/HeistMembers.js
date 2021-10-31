import React, { Component } from 'react';
import { Redirect } from 'react-router';
import { HeistDetails } from './HeistDetails';

export class HeistMembers extends Component {
    static displayName = HeistMembers.name;

    constructor(props) {
        super(props);
        this.state = {
            members: [], loading: true, redirect: false
        };
    }

    componentDidMount() {
        this.populateMemberData();
    }

    handleOnClick = () => {
        this.setState({ redirect: true});
    }

    static renderHeistMembers(members, handleOnClick) {
        return (
            <div>
                <h1>Heist</h1>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Skills</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            members.length > 0 ?
                                members.map(member =>
                                    <tr>
                                        <td>{member.name}</td>
                                        <td>{member.skills.map(skill => <p>{skill.name}: {skill.level}</p>)}</td>
                                    </tr>
                                ) : <td colSpan="5" className='text-center'>No members currently</td>
                        }
                    </tbody>
                </table>
                <br />
                <button className="btn btn-primary" onClick={() => handleOnClick()}>Heist Details</button>
            </div>
        );
    }

    render() {
        if (this.state.redirect) {
            //<Redirect
            //    to={{
            //        pathname: "heist/" + this.props.location.state.heistId + "/members",
            //        state: { heistId: this.props.location.state.heistId }
            //    }}
            ///>
            return (<HeistDetails heistId={this.props.heistId} />);
        }
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : HeistMembers.renderHeistMembers(this.state.members, this.handleOnClick)

        return (
            <div>
                {contents}
            </div>
        );
    }

    async populateMemberData() {
        const response = await fetch('heist/' + this.props.heistId + '/members');
        const data = await response.json();
        this.setState({ members: data, loading: false });
    }
}
