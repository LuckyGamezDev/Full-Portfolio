function startSession() {
    const sessionNameInputField = document.getElementById("session-start-name");
    const stopTimerButton = document.getElementById("stop-timer-button");

    let sessionsArray = getAllSessionsArray();
    let shouldTimerBeActive = false;
    let isTimerActive = false;

    let sessionToStart = null;
    for (let session of sessionsArray) {
        if (session.name == sessionNameInputField.value) {
            sessionToStart = session;

            break;
        }
    }

    if (sessionToStart) {
        countdown(sessionToStart.length);
    } else {
        alert("This session does not (yet) exist. Please specify an other session or create a new one!");

        return;
    }
}
    let mins = .1;
    let secs = mins * 60;

    function countdown(countdownLengthMinutes) {
        secs = countdownLengthMinutes * 60;
        mins = getMinutes();

        setTimeout('decrement()', 60);

        stopTimerButton.style.visibility = true;
    }

    // Got this code from someone's article
    function decrement() {
        if (shouldTimerBeActive) {
            isTimerActive = true;
        } else {
            isTimerActive = false;

            return;
        }

        while (isTimerActive) {
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
                } else {
                    secs--;
                    setTimeout('decrement()', 1000);
                }
            }
        }
    }

    function stopTimer() {
        shouldTimerBeActive = false;

        stopTimerButton.style.visibility = false;
    }

    function getMinutes() {
        mins = Math.floor(secs / 60);
        return mins;
    }

    function getSeconds() {
        return secs - Math.round(mins * 60);
    }