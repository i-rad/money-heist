import React, { Component } from 'react';
import { Redirect } from 'react-router';
import { HeistDetails } from './HeistDetails';
import { CreateHeist } from './CreateHeist';

export class Heists extends Component {
    static displayName = Heists.name;

    constructor(props) {
        super(props);
        this.state = { heists: [], loading: true, redirect: false, heistId: 0, createRedirect: false };

        //this.routeChange = this.routeChange.bind(this);
    }

    componentDidMount() {
        this.populateHeistData();
    }

    handleOnClick = (heistId) => {
        this.setState({ redirect: true, heistId: heistId });
    }

    handleOnCreateClick = () => {
        this.setState({ createRedirect: true });
    }

    render() {
        if (this.state.redirect) {
            //return (<Redirect
            //    to={{
            //        pathname: "heist/" + this.state.heistId,
            //        state: { heistId: this.state.heistId }
            //    }}
            ///>);
            return (<HeistDetails heistId={this.state.heistId} />);
        }

        if (this.state.createRedirect) {
            return (<CreateHeist />);
        }

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <table className='table table-striped' aria-labelledby="tabelLabel">
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
                        this.state.heists.length > 0 ?
                            this.state.heists.map(heist =>
                                <tr key={heist.heistId}>
                                    <td>{heist.name}</td>
                                    <td>{heist.location}</td>
                                    <td>{heist.startTime.replace("T", " ")}</td>
                                    <td>{heist.endTime.replace("T", " ")}</td>
                                    <td>{heist.heistStatus.name}</td>
                                    <td>
                                        <button color="primary" className="px-4 btn btn-primary"
                                            onClick={() => this.handleOnClick(heist.heistId)}
                                        >
                                            Details
                                        </button></td>
                                </tr>
                            ) : <td colSpan="5" className='text-center'>No heists currently</td>
                    }
                </tbody>
            </table>

        return (
            <div>
                <div className='d-flex justify-content-between mb-3'>
                    <h1 id="tabelLabel" >All heists</h1>
                    <button color="primary" className="px-4 btn btn-primary"
                        onClick={() => this.handleOnCreateClick()}
                    >
                        Add new heist
                    </button>
                </div>
                {contents}
            </div>
        );
    }

    async populateHeistData() {
        const response = await fetch('heist');
        const data = await response.json();
        this.setState({ heists: data, loading: false });
    }
}
