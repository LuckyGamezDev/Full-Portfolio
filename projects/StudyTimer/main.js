const maxSessionNameLength = 30

var sessionsArray = [];

var sessionToAdd = null;

document.addEventListener("DOMContentLoaded", function() {
    sessionsArray = JSON.parse(localStorage.getItem("user-sessions"));

    for (let session of sessionsArray) {
        addSessionToContainer(session);
    }
});

function addSessionToContainer(session) {
    const sessionsContainer = document.querySelector(".sessions-container");

    if (sessionToAdd && sessionsContainer) {
        sessionsContainer.appendChild(sessionToAdd);

        sessionToAdd = null;
    }

    let sessionNodeContainer = document.createElement("div");

    let sessionNodeName = document.createElement("a");
    let sessionNodeLength = document.createElement("a");

    sessionNodeName.innerHTML = "session name: " + session.name + " ";
    sessionNodeLength.innerHTML = "session length: " + session.length + " ";

    sessionNodeContainer.appendChild(sessionNodeName);
    sessionNodeContainer.appendChild(sessionNodeLength);

    if (sessionsContainer) {                        
        sessionsContainer.appendchild(sessionNodeContainer);
    } else {
        sessionToAdd = sessionNodeContainer;
    }
                    
}

function createSession() {
    if (sessionsArray == null) {
        sessionsArray = [];
    }

    let sessionNameInputField = document.getElementById("session-name");
    let sessionLengthInputField = document.getElementById("session-length");

        if (sessionNameInputField.value == null || sessionLengthInputField == null) {
            console.log("values are not valid!");
            return;
        }

        let createdSession = new Session(sessionNameInputField.value, sessionLengthInputField.value);

        if (validateSession(createdSession) == true) {
            sessionsArray.push(createdSession);

            console.log("Created new session. Current sessions:");
            for (let session of sessionsArray) {
                console.log(session.name + " " + session.length);
            }

            addSessionToContainer(createdSession)
                    
            localStorage.setItem("user-sessions", JSON.stringify(sessionsArray));
        }

        sessionNameInputField.value = "";
        sessionLengthInputField.value = "";
}

function validateSession(session) {
    if (session.name == null || session.name == "") {
        alert("Please fill in a name for this session!");

        return false;
    } else if (session.length == null || session.length == "") {
        alert("Please fill in the length of the session!");

        return false;
    }

    if (session.name.length > maxSessionNameLength) {
        alert("Session name can't be longer than " + maxSessionNameLength);

        return false;
    }

    if (session.length > 60) {
        return false;
    }

    return true;
}

class Session {
    constructor(name, length) {
        this.name = name;
        this.length = length;
    }
}