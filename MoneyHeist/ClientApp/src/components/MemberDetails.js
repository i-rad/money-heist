import React, { Component } from 'react';
import { Members } from './Members';
import { UpdateSkills } from './UpdateSkills';

export class MemberDetails extends Component {
    static displayName = MemberDetails.name;

    constructor(props) {
        super(props);
        this.state = {
            member: {}, loading: true, redirect: false, createRedirect: false
        };
    }

    componentDidMount() {
        this.populateMemberData();
    }

    handleOnClick = () => {
        this.setState({ redirect: true });
    }

    handleOnDeleteClick = async (skillName) => {
        const response = await fetch('member/' + this.props.memberId + "/skills/" + skillName, { method: 'DELETE' });
        this.state.member.skills.splice(this.state.member.skills.findIndex(item => item.name === skillName), 1);
        this.setState({ member: this.state.member, loading: false });
    }

    handleOnCreateClick = () => {
        this.setState({ createRedirect: true });
    }

    static renderMemberDetails(member, handleOnClick, handleOnDeleteClick, handleOnCreateClick) {
        return (
            <div>
                <h1>Heist</h1>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Sex</th>
                            <th>Email</th>
                            <th>Status</th>
                            <th>Main Skill</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            <tr>
                                <td>{member.name}</td>
                                <td>{member.sex}</td>
                                <td>{member.email}</td>
                                <td>{member.status}</td>
                                <td>{member.mainSkill}</td>
                            </tr>
                        }
                    </tbody>
                </table>
                <br />
                <div className='d-flex justify-content-between mb-3'>
                    <h1 id="tabelLabel" >Skills</h1>
                    <button color="primary" className="px-4 btn btn-primary"
                        onClick={() => handleOnCreateClick()}
                    >
                        Update skills
                    </button>
                </div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Level</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            member.skills.length > 0 ?
                                member.skills.map(skill =>
                                    <tr>
                                        <td>{skill.name}</td>
                                        <td>{skill.level}</td>
                                        {
                                            member.skills.length > 1 ?
                                            <td>
                                                <button className="btn btn-primary mr-2" onClick={() => handleOnDeleteClick(skill.name)}>Delete</button>
                                            </td> : null

                                        }
                                    </tr>
                                ) :<tr><td colSpan="5" className='text-center'>No skills currently</td></tr>
                        }
                    </tbody>
                </table>
                <button className="btn btn-primary mr-2" onClick={() => handleOnClick()}>Back to all members</button>

            </div>
        );
    }

    render() {
        if (this.state.redirect) {
            return (<Members />);
        }

        if (this.state.createRedirect) {
            return (<UpdateSkills memberId={this.props.memberId} />);
        }

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : MemberDetails.renderMemberDetails(this.state.member, this.handleOnClick, this.handleOnDeleteClick, this.handleOnCreateClick)

        return (
            <div>
                {contents}
            </div>
        );
    }

    async populateMemberData() {
        const response = await fetch('member/' + this.props.memberId);
        const data = await response.json();
        this.setState({ member: data, loading: false });
    }
}
