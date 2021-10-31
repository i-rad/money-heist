import React, { Component } from 'react';

export class CreateMember extends React.Component {
    constructor(props) {
        super(props);
        this.state = { name: '', sex: 'F', email: '', mainSkill: '', status: 'AVAILABLE', skills: [{ name: "", level: "" }], fields: [{ value: null }] };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        const target = event.target;
        const value = target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }

    handleSkillChange(i, event) {
        const target = event.target;
        const name = target.name;
        // level validation
        if (name == "level") {
            target.value = target.value.replace(/./g, '*');
            if (target.value.length > 10) {
                target.value = target.value.substr(0, 10);
            }
        }
        if (this.state.skills[i] != null) {
            this.state.skills[i][name] = target.value;
        }
        else {
            if (name == "name") {
                this.state.skills.push({ name: target.value, level: "" })
            }
            else {
                this.state.skills.push({ name: "", level: target.value })
            }
        }
        this.setState({
            skills: this.state.skills
        });
    }

    handleAdd() {
        this.state.fields.push({ value: null });
        this.setState({
            fields: this.state.fields
        });
    }

    handleRemove(i) {
        this.state.fields.splice(i, 1)
        this.setState({
            fields: this.state.fields
        });
    }

    handleSubmit = async () => {
        delete this.state.fields;
        const response = await fetch('member', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(this.state),
        });
        if (response.status == 201) {
            alert("Created!");
        }
        else {
            alert("Something went wrong!");
        }
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit} style={{ display: "grid" }}>
                <label>
                    Name:
                    <input className='form-control' name="name" type="text" value={this.state.value} onChange={this.handleChange} required />
                </label>
                <label>
                    Sex:
                    <select className='form-control' name="sex" value={this.state.value} onChange={this.handleChange}>
                        <option value="F">F</option>
                        <option value="M">M</option>
                    </select>
                </label>
                <label>
                    Email:
                    <input className='form-control' name="email" type="email" value={this.state.value} onChange={this.handleChange} required />
                </label>
                <label>
                    Main skill:
                    <input className='form-control' name="mainSkill" type="text" value={this.state.value} onChange={this.handleChange} />
                </label>
                <label>
                    Status:
                    <select className='form-control' name="status" value={this.state.value} onChange={this.handleChange}>
                        <option value="AVAILABLE">AVAILABLE</option>
                        <option value="EXPIRED">EXPIRED</option>
                        <option value="INCARCERATED">INCARCERATED</option>
                        <option value="RETIRED">RETIRED</option>
                    </select>
                </label>

                <button type="button" className="w-50 mb-2 btn btn-secondary" onClick={() => this.handleAdd()}>
                    Add skill
                </button>

                {this.state.fields.map((field, idx) => {
                    return (
                        <div className='d-flex mb-2' key={`${field}-${idx}`}>
                            <input className='form-control'
                                type="text"
                                placeholder="Enter text"
                                name="name"
                                onChange={e => this.handleSkillChange(idx, e)}
                                required
                            />
                            <input className='form-control'
                                type="text"
                                name="level"
                                placeholder="Add stars to represent level of skill"
                                onChange={e => this.handleSkillChange(idx, e)}
                                required
                            />
                            {
                                idx > 0 ?
                                    <button type="button" className='ml-2 btn btn-secondary' onClick={() => this.handleRemove(idx)}>
                                        X
                                    </button> : null
                            }
                        </div>
                    );
                })}

                <input type="submit" value="Submit" className='btn btn-secondary' />
            </form>
        );
    }
}