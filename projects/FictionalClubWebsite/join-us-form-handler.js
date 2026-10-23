var firstNameField;
var lastNameField;
var emailField;
var birthdateField;

var firstnameFieldInvalidTag;
var lastnameFieldInvalidTag;
var emailFieldInvalidTag;
var birthdateFieldInvalidTag;

document.addEventListener('DOMContentLoaded', function() {
    firstNameField = document.querySelector(".submission #submission-form #first-name");
    lastNameField = document.querySelector(".submission #submission-form #last-name");
    emailField = document.querySelector(".submission #submission-form #user-email");
    birthdateField = document.querySelector(".submission #submission-form #user-birthdate");

    firstnameFieldInvalidTag = document.querySelector(".submission #submission-form #invalid-first-name-label"); 
    lastnameFieldInvalidTag = document.querySelector(".submission #submission-form #invalid-last-name-label"); 
    emailFieldInvalidTag = document.querySelector(".submission #submission-form #invalid-user-email-label"); 
    birthdateFieldInvalidTag = document.querySelector(".submission #submission-form #invalid-birthdate-label"); 

    firstNameField.addEventListener('change', onFirstNameFieldChanged);
    lastNameField.addEventListener('change', onLastNameFieldChanged);
    emailField.addEventListener('change', onEmailChanged);
    birthdateField.addEventListener('change', onBirthdateChanged);
});

function onFirstNameFieldChanged() {
    console.log("Checking user name...");

    var regex = /\d/;
    if (regex.test(firstNameField.value)) {
        firstnameFieldInvalidTag.style.visibility = "visible";
    } else {
        firstnameFieldInvalidTag.style.visibility = "hidden";
    }
}

function onLastNameFieldChanged() {
    var regex = /\d/;
    if (regex.test(lastNameField.value)) {
        lastnameFieldInvalidTag.style.visibility = "visible";
    } else {
        lastnameFieldInvalidTag.style.visibility = "hidden";
    }
}

function onEmailChanged() {
    if (!emailField.value.includes("@gmail.com")) {
        emailFieldInvalidTag.style.visibility = "visible";
    } else {
        emailFieldInvalidTag.style.visibility = "hidden";
    }
}

function onBirthdateChanged() {
    selectedDate = this.value; // Value is in yyyy-mm-dd format

    selectedDate.split('-');

    if (2026 - selectedDate[0] < 16) { // Use 2026 (at least for now) since it is currently that year. Might add a current year check
        console.log("invalid birthdate!")
        birthdateFieldInvalidTag.style.visibility = "visible";
    } else {
        birthdateFieldInvalidTag.style.visibility = "hidden";
    }
}