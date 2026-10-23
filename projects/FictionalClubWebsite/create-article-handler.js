var titleInputText;
var titleRemainingMaxLengthText;
var titleInvalidTag;

const titleInputTextId = "#article-title #article-title-text";
const titleRemainingMaxLengthTextId = "#article-title #max-length-previewer";
const titleInvalidTagId = "#article-title #invalid-label";

var bodyInputText;
var bodyRemainingMaxLengthText;
var bodyInvalidTag;

const bodyInputTextId = "#article-body #article-body-text";
const bodyRemainingMaxLengthTextId = "#article-body #max-length-previewer";
const minValidBodyLength = 300;
const bodyInvalidTagId = "#article-body #invalid-label";

var sourcesInputText;
var sourcesRemainingMaxLengthText;
var sourcesInvalidTag;

const sourcesInputTextId = "#article-sources #article-sources-text";
const sourcesRemainingMaxLengthTextId = "#article-sources #max-length-previewer";
const sourcesInvalidTagId = "#article-sources #invalid-label";

isTitleValid = false;
isBodyValid = false;
isSourceValid = false;

document.addEventListener("DOMContentLoaded", function() {
    titleInputText = document.querySelector(titleInputTextId);
    titleRemainingMaxLengthText = document.querySelector(titleRemainingMaxLengthTextId);
    titleInvalidTag = document.querySelector(titleInvalidTagId);

    titleInvalidTag.style.visibility = "hidden";

    titleInputText.addEventListener("input", function() {
        displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);

        if (titleInputText.value.length > 0) {
            isTitleValid = true;

            titleInvalidTag.style.visibility = "hidden";
        } else {
            isTitleValid = false;
        }
    });

    displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);

    bodyInputText = document.querySelector(bodyInputTextId);
    bodyRemainingMaxLengthText = document.querySelector(bodyRemainingMaxLengthTextId);
    bodyInvalidTag = document.querySelector(bodyInvalidTagId);

    bodyInvalidTag.style.visibility = "hidden";

    bodyInputText.addEventListener("input", function() {
        displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);

        if (bodyInputText.value.length >= minValidBodyLength) {
            isBodyValid = true;

            bodyInvalidTag.style.visibility = "hidden";
        } else {
            isBodyValid = false;
        }
    });

    bodyInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);

    sourcesInputText = document.querySelector(sourcesInputTextId);
    sourcesRemainingMaxLengthText = document.querySelector(sourcesRemainingMaxLengthTextId);    
    sourcesInvalidTag = document.querySelector(sourcesInvalidTagId);

    sourcesInvalidTag.style.visibility = "hidden";

    sourcesInputText.addEventListener("input", function() {
        displayTextLengthLeft(sourcesInputTextId, sourcesRemainingMaxLengthTextId);

        if (sourcesInputText.value.length > 0) {
            isSourceValid = true;

            sourcesInvalidTag.style.visibility = "hidden";
        } else {
            isSourceValid = false;
        }
    })

    sourcesInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(sourcesInputTextId, sourcesRemainingMaxLengthTextId);
});

function displayTextLengthLeft(textBoxId, textLeftDisplayId, isTextField = true) {
    if (isTextField) {
        var bodyInputText = document.querySelector(textBoxId);
        var remainingMaxLengthText = document.querySelector(textLeftDisplayId);

        var remainingLettersAmount = bodyInputText.maxLength - bodyInputText.textLength;

        remainingMaxLengthText.innerHTML = "Remaining letters: " + remainingLettersAmount;

        if (remainingLettersAmount == 0) {
            remainingMaxLengthText.style.color = "red";
        } else {
            remainingMaxLengthText.style.color = "black";
        }
    } else {
        var bodyInputText = document.querySelector(textBoxId);
        var remainingMaxLengthText = document.querySelector(textLeftDisplayId);

        var remainingLettersAmount = bodyInputText.maxLength - bodyInputText.value.length;

        remainingMaxLengthText.innerHTML = "Remaining letters: " + remainingLettersAmount;

        if (remainingLettersAmount == 0) {
            remainingMaxLengthText.style.color = "red";
        } else {
            remainingMaxLengthText.style.color = "black";
        }
    }
}

function autoResizeTextArea() {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
}

function createArticle() {
    if (isTitleValid && isBodyValid && isSourceValid) {
        articleTitle = titleInputText.value;
        articleBody = bodyInputText.value;
        articleSources = sourcesInputText.value;

        console.log("created article!")

        window.open("community.html", "_top");
    } else {
        console.log("Cannot create article, please input something into the fields!")

        if (!isTitleValid) {
            titleInvalidTag.style.visibility = "visible"
        }
        if (!isBodyValid) {
            bodyInvalidTag.style.visibility = "visible";
        }
        if (!isSourceValid) {
            sourcesInvalidTag.style.visibility = "visible";
        }
    }
}