let shouldTimerBeActive = false;

let sessionToStart = null;

let createdSessionsArray = [];
let mins;
let secs;

function startSession() {
    stopSession();

    createdSessionsArray = getAllSessionsArray();

    const sessionNameInputField = document.getElementById("session-start-name");

    for (let session of createdSessionsArray) {
        if (session.name == sessionNameInputField.value) {

            sessionToStart = session;

            break;
        }
    }

    if (sessionToStart) {
        setTimeout(function() {
            countdown(sessionToStart.length);
        }, 1000);
    } else {
        alert("This session does not (yet) exist. Please specify an other session or create a new one!");

        return;
    }
}

function countdown(countdownLengthMinutes) {
    secs = countdownLengthMinutes * 60;
    mins = getMinutes();

    shouldTimerBeActive = true;

    setTimeout('decrement()', 60);
}

// Got this code from someone's article
function decrement() {
    while (shouldTimerBeActive) {
        if (document.getElementById) { // I wonder what this is for
            timerMinutes = document.getElementById("timer-minutes");
            timerSeconds = document.getElementById("timer-seconds");

            if (timerSeconds < 59) {
                timerSeconds.value = secs;
            } else {
                timerMinutes.value = getMinutes();
                timerSeconds.value = getSeconds();
            }

            if (mins < 1) {
                timerMinutes.style.color = "red";
                timerSeconds.style.color = "red";
            }

            if (mins < 0) {
                alert('time is up!');

                timerMinutes.value = 0;
                timerSeconds.value = 0;

                shouldTimerBeActive = false;

                return;
            } else {
                secs--;

                setTimeout('decrement()', 1001);

                break;
            }
        }
    }
}

function stopSession() {
    shouldTimerBeActive = false;

    timerMinutes = document.getElementById("timer-minutes");
    timerSeconds = document.getElementById("timer-seconds");

    timerMinutes.value = 0;
    timerSeconds.value = 0;
}

function getMinutes() {
    mins = Math.floor(secs / 60);
    return mins;
}

function getSeconds() {
    return secs - Math.round(mins * 60);
}