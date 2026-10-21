var titleInputText;
var titleRemainingMaxLengthText;

const titleInputTextId = "#article-title #article-title-text";
const titleRemainingMaxLengthTextId = "#article-title #max-length-previewer";

var bodyInputText;
var bodyRemainingMaxLengthText;

const bodyInputTextId = "#article-body #article-body-text";
const bodyRemainingMaxLengthTextId = "#article-body #max-length-previewer";

var sourcesInputText;
var sourcesRemainingMaxLengthText;

const sourcesInputTextId = "#article-sources #article-sources-text";
const sourcesRemainingMaxLengthTextId = "#article-sources #max-length-previewer";

document.addEventListener("DOMContentLoaded", function() {
    titleInputText = document.querySelector(titleInputTextId);
    titleRemainingMaxLengthText = document.querySelector(titleRemainingMaxLengthTextId);

    titleInputText.addEventListener("input", function(){
        displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);
    });

    // titleInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);

    bodyInputText = document.querySelector(bodyInputTextId);
    bodyRemainingMaxLengthText = document.querySelector(bodyRemainingMaxLengthTextId);

    bodyInputText.addEventListener("input", function() {
        displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);
    });

    // bodyInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);

    sourcesInputText = document.querySelector(sourcesInputTextId);
    sourcesRemainingMaxLengthText = document.querySelector(sourcesRemainingMaxLengthTextId);    

    sourcesInputText.addEventListener("input", function() {
        displayTextLengthLeft(sourcesInputTextId, sourcesRemainingMaxLengthTextId);
    })

    // sourcesInputText.addEventListener("input", autoResizeTextArea, false);

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

    function autoResizeTextArea() {
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    }
}