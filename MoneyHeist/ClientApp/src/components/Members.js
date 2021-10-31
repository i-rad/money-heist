import React, { Component } from 'react';
import { Redirect } from 'react-router';
import { MemberDetails } from './MemberDetails';
import { CreateMember } from './CreateMember';

export class Members extends Component {
    static displayName = Members.name;

    constructor(props) {
        super(props);
        this.state = { members: [], loading: true, redirect: false, memberId: 0, createRedirect: false };

        //this.routeChange = this.routeChange.bind(this);
    }

    componentDidMount() {
        this.populateMembersData();
    }

    handleOnClick = (memberId) => {
        this.setState({ redirect: true, memberId: memberId });
    }

    handleOnCreateClick = () => {
        this.setState({ createRedirect: true});
    }


    render() {

        if (this.state.redirect) {
            return (<MemberDetails memberId={this.state.memberId} />);
        }

        if (this.state.createRedirect) {
            return (<CreateMember />);
        }

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Sex</th>
                        <th>Email</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.state.members.length > 0 ?
                            this.state.members.map(member =>
                                <tr key={member.memberId}>
                                    <td>{member.name}</td>
                                    <td>{member.sex}</td>
                                    <td>{member.email}</td>
                                    <td>{member.memberStatus.name}</td>
                                    <td>
                                        <button color="primary" className="px-4 btn btn-primary"
                                            onClick={() => this.handleOnClick(member.memberId)}
                                        >
                                            Details
                                        </button></td>
                                </tr>
                            ) : <td colSpan="5" className='text-center'>No members currently</td>
                    }
                </tbody>
            </table>

        return (
            <div>
                <div className='d-flex justify-content-between mb-3'>
                    <h1 id="tabelLabel" >All members</h1>
                    <button color="primary" className="px-4 btn btn-primary"
                        onClick={() => this.handleOnCreateClick()}
                    >
                        Add new member
                    </button>
                </div>
                {contents}
            </div>
        );
    }

    async populateMembersData() {
        const response = await fetch('member');
        const data = await response.json();
        this.setState({ members: data, loading: false });
    }
}
