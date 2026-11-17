var titleInputText;
var titleRemainingMaxLengthText;
var titleInvalidTag;

const titleInputTextId = "#article-title #article-title-text";
const titleRemainingMaxLengthTextId = "#article-title .max-length-previewer";
const titleInvalidTagId = "#article-title .invalid-label";

var bodySummaryInputText;
var bodySummaryRemainingMaxLengthText;
var bodySummaryInvalidTag;

const bodySummaryInputTextId = "#article-body-summary #article-body-summary-text";
const bodySummaryRemainingMaxLengthTextId = "#article-body-summary .max-length-previewer";
const bodySummaryInputTextMinLength = 50;
const bodySummaryInvalidTagId = "#article-body-summary .invalid-label";

var bodyInputText;
var bodyRemainingMaxLengthText;
var bodyInvalidTag;

const bodyInputTextId = "#article-body #article-body-text";
const bodyRemainingMaxLengthTextId = "#article-body .max-length-previewer";
const minValidBodyLength = 1 //300;
const bodyInvalidTagId = "#article-body .invalid-label";

var metadataInputText;
var metadataRemainingMaxLengthText;
var metadataInvalidTag;

const metadataInputTextId = "#article-metadata #article-metadata-text";
const metadataRemainingMaxLengthTextId = "#article-metadata .max-length-previewer";
const metadataInvalidTagId = "#article-metadata .invalid-label";

var isTitleValid = false;
var isBodySummaryValid = false;
var isBodyValid = false;
var isMetadataValid = false;


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

    bodySummaryInputText = document.querySelector(bodySummaryInputTextId);
    bodySummaryRemainingMaxLengthText = document.querySelector(bodySummaryRemainingMaxLengthTextId);
    bodySummaryInvalidTag = document.querySelector(bodySummaryInvalidTagId);

    bodySummaryInvalidTag.style.visibility = "hidden";

    bodySummaryInputText.addEventListener("input", function() {
        displayTextLengthLeft(bodySummaryInputTextId, bodySummaryRemainingMaxLengthTextId);

        if (bodySummaryInputText.value.length >= bodySummaryInputTextMinLength) {
            isBodySummaryValid = true;

            bodySummaryInvalidTag.style.visibility = "hidden";
        } else {
            isBodySummaryValid = false;
        }
    });

    bodySummaryInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(bodySummaryInputTextId, bodySummaryRemainingMaxLengthTextId);


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

    metadataInputText = document.querySelector(metadataInputTextId);
    metadataRemainingMaxLengthText = document.querySelector(metadataRemainingMaxLengthTextId);    
    metadataInvalidTag = document.querySelector(metadataInvalidTagId);

    metadataInvalidTag.style.visibility = "hidden";

    metadataInputText.addEventListener("input", function() {
        displayTextLengthLeft(metadataInputTextId, metadataRemainingMaxLengthTextId);

        if (metadataInputText.value.length > 0) {
            isMetadataValid = true;

            metadataInvalidTag.style.visibility = "hidden";
        } else {
            isMetadataValid = false;
        }
    })

    metadataInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(metadataInputTextId, metadataRemainingMaxLengthTextId);

    document.getElementById("publish-article").onclick = () => {
        createArticle();
    }
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

function makeSelectionStyled(startTag, endTag) {
    const textArea = bodyInputText;
    const textAreaSelectionStart = textArea.selectionStart;
    const textAreaSelectionEnd = textArea.selectionEnd;
    const textAreaText = textArea.value;
    const isHeader = startTag.includes("#");

    // Get the selected text
    const selectedText = textAreaText.substring(textAreaSelectionStart, textAreaSelectionEnd);

    // Replacing the selected text with the wanted formatted form

    var formattedText;
    if (endTag) {
        formattedText = startTag + selectedText + endTag;
    } else { 
        formattedText = startTag + selectedText;
    }

    if (isHeader) { // Auto add white lines at the start of the header to make it more polished somehow, later
        textArea.value = textAreaText.substring(0, textAreaSelectionStart) + formattedText + "\r\n\r\n" + textAreaText.substring(textAreaSelectionEnd);
    } else {
        textArea.value = textAreaText.substring(0, textAreaSelectionStart) + formattedText + textAreaText.substring(textAreaSelectionEnd);
    }    

    //autoResizeTextArea();

    // Reset the cursor position
    textArea.selectionStart = textAreaSelectionStart;
    textArea.selectionEnd = textAreaSelectionEnd;

    // Bring back focus on the text area so that the user can continue typing
    textArea.focus();

    console.log("Styled text");
}

async function createArticle() {
    if (isTitleValid && isBodySummaryValid && isBodyValid && isMetadataValid) {
        var articleTitle = titleInputText.value;
        var articleBodySummary = bodySummaryInputText.value;
        var articleBody = bodyInputText.value;
        var articleMetadata = metadataInputText.value;

        var createdArticle = new Article(articleTitle, articleBodySummary, articleBody, articleMetadata);
        console.log("Variable value: " + articleBodySummary, " The saved class value: " + createdArticle.bodySummary);
        var jsonStringArticle = createdArticle.convertToJson();

        console.log("Sending this article: " + jsonStringArticle);

        if (isTitleValid && isBodySummaryValid && isBodyValid && isMetadataValid) {

    const articleTitle = titleInputText.value;
    const articleBodySummary = bodySummaryInputText.value;
    const articleBody = bodyInputText.value;
    const articleMetadata = metadataInputText.value;

    const createdArticle = new Article(
        articleTitle,
        articleBodySummary,
        articleBody,
        articleMetadata
    );

    const jsonStringArticle = createdArticle.convertToJson();

    try {
        const response = await fetch("http://localhost:3590/articles", {
            method: "POST",
            body: jsonStringArticle,
            headers: { "Content-Type": "application/json" }
        });

        if (!response.ok) {
            throw new Error(`HTTP error: ${response.status}`);
        }

        console.log("Article successfully created!");

        window.open("community.html", "_self");

    } catch (err) {
        console.error("ERR:", err);
    }
}

    } else {
        console.log("Cannot create article, please input something into the fields!")

        if (!isTitleValid) {
            titleInvalidTag.style.visibility = "visible"
        }
        if (!isBodySummaryValid) {
            bodySummaryInvalidTag.style.visibility = "visible";
        }
        if (!isBodyValid) {
            bodyInvalidTag.style.visibility = "visible";
        }
        if (!isMetadataValid) {
            metadataInvalidTag.style.visibility = "visible";
        }
    }
}

class Article {
    constructor(title, bodySummary, body, metadata) {
        this.title = title;
        this.bodySummary = bodySummary,
        this.body = body;
        this.metadata = metadata;
    }

    convertToJson() {
        const jsonObject = {
            "title": this.title,
            "body-summary": this.bodySummary,
            "body": this.body,
            "metadata": this.metadata,
            "date": new Date().toDateString()
        };

        console.log(jsonObject["body"]);

        return JSON.stringify(jsonObject);
    }
}