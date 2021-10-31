import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div className='text-center'>
        <h1>Hello, world!</h1>
            <h3>Welcome to my new application.</h3>
            <h3>Take your pick:</h3>
            <Link to={`/heists`} className="btn btn-secondary btn-lg m-5 p-5" style={{width: "20%"}}>Heists</Link>
            <Link to={`/members`} className="btn btn-secondary btn-lg m-5 p-5" style={{ width: "20%" }}>Members</Link>
      </div>
    );
  }
}
