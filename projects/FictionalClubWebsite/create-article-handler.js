var titleInputText;
var titleRemainingMaxLengthText;

var bodyInputText;
var bodyRemainingMaxLengthText;

document.addEventListener("DOMContentLoaded", function() {
    titleInputText = document.querySelector("#article-title #article-title-text");
    titleRemainingMaxLengthText = document.querySelector("#article-title #max-length-previewer");

    titleInputText.addEventListener("input", displayTitleTextTooLong);
    displayTitleTextTooLong();

    bodyInputText = document.querySelector("#article-body #article-body-text")
    bodyRemainingMaxLengthText = document.querySelector("#article-body #max-length-previewer")

    bodyInputText.addEventListener("input", displayBodyTextLengthLeft);
 
    displayBodyTextLengthLeft();
});

function displayTitleTextTooLong() {
    var remainingLettersAmount = titleInputText.maxLength - titleInputText.value.length;

    titleRemainingMaxLengthText.innerHTML = "Remaining letters: " + remainingLettersAmount;
}

function displayBodyTextLengthLeft() {
    var remainingLettersAmount = bodyInputText.maxLength - bodyInputText.textLength;

    bodyRemainingMaxLengthText.innerHTML = "Remaining letters: " + remainingLettersAmount;
}