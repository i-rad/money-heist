import React, { Component } from 'react';

export class UpdateHeistSkills extends React.Component {
    constructor(props) {
        super(props);
        this.state = { skills: [{ name: "", level: "", member: 0 }], fields: [{ value: null }] };

        this.handleSubmit = this.handleSubmit.bind(this);
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
            if (name == "members") {
                this.state.skills[i][name] = Number(target.value);
            }
            else {
                this.state.skills[i][name] = target.value;
            }
        }
        else {
            if (name == "name") {
                this.state.skills.push({ name: target.value, level: "", members: 0 })
            }
            else if (name == "level") {
                this.state.skills.push({ name: "", level: target.value, members: 0 })
            }
            else {
                this.state.skills.push({ name: "", level: "", members: Number(target.value) })
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
        var requestBody = this.state;
        delete requestBody.fields;
        const response = await fetch('heist/' + this.props.heistId + "/skills", {
            method: 'PATCH',
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

    render() {
        return (
            <form onSubmit={this.handleSubmit} style={{ display: "grid" }}>

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
                            <input className='form-control'
                                type="number"
                                name="members"
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