const maxSessionNameLength = 30

var sessionsArray = [];

var sessionToAdd = null;

document.addEventListener("DOMContentLoaded", function() {
    sessionsArray = JSON.parse(localStorage.getItem("user-sessions"));

    if (sessionsArray == null) {
        sessionsArray = [];
    }

    loadAllSessionsIntoContainer();
});

function loadAllSessionsIntoContainer() {
    for (let session of sessionsArray) {
        addSessionToContainer(session);
    }
}

function addSessionToContainer(session) {
    const sessionsContainer = document.getElementById("sessions-holder-container");

    // If for some reason the last session that was supposed to be added wasn't added properly, add it here
    if (sessionToAdd && sessionsContainer) {
        sessionsContainer.appendChild(sessionToAdd);

        sessionToAdd = null;
    }

    let sessionNodeContainer = document.createElement('div');
    sessionNodeContainer.id = "session-container-instance";

    let sessionNodeName = document.createElement('a');
    sessionNodeName.id = "session-container-instance-name";

    let sessionNodeLength = document.createElement('a');
    sessionNodeLength.id = "session-container-instance-length";

    sessionNodeName.innerHTML = "session name: " + session.name + " ";
    sessionNodeLength.innerHTML = "session length: " + session.length + " ";

    sessionNodeContainer.appendChild(sessionNodeName);
    sessionNodeContainer.appendChild(sessionNodeLength);

    if (sessionsContainer) {                        
        sessionsContainer.appendChild(sessionNodeContainer);
    } else {
        // If for some reason this happens, make this session be assigned the next time a new one gets created as fail safe
        sessionToAdd = sessionNodeContainer;
    }
                    
}

function createSession() {
    if (sessionsArray == null) {
        sessionsArray = [];
    }

    let sessionNameInputField = document.getElementById("session-create-name");
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

function clearAllSessions() {
    localStorage = null;

    sessionsArray = [];

    const sessionsContainer = document.getElementById("sessions-holder-container");

    while (sessionsContainer.firstChild) {
        sessionsContainer.removeChild(sessionsContainer.lastChild);
    }

    loadAllSessionsIntoContainer();
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

    for (let sessionFromArray of sessionsArray) {
        if (sessionFromArray.name == session.name) {
            alert("This isn't a valid session name since an other session already exists with this given name!");

            return false;
        }
    }

    return true;
}

function getAllSessionsArray() {
    return sessionsArray;
}

class Session {
    constructor(name, length) {
        this.name = name;
        this.length = length;
    }
}