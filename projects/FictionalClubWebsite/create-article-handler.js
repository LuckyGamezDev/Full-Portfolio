let titleInputText;
let titleRemainingMaxLengthText;
let titleInvalidTag;

const titleInputTextId = "#article-title #article-title-text";
const titleRemainingMaxLengthTextId = "#article-title .max-length-previewer";
const titleInvalidTagId = "#article-title .invalid-label";

let bodySummaryInputText;
let bodySummaryRemainingMaxLengthText;
let bodySummaryInvalidTag;

const bodySummaryInputTextId = "#article-body-summary #article-body-summary-text";
const bodySummaryRemainingMaxLengthTextId = "#article-body-summary .max-length-previewer";
const bodySummaryInputTextMinLength = 50;
const bodySummaryInvalidTagId = "#article-body-summary .invalid-label";

let bodyInputText;
let bodyRemainingMaxLengthText;
let bodyInvalidTag;

const bodyInputTextId = "#article-body #article-body-text";
const bodyRemainingMaxLengthTextId = "#article-body .max-length-previewer";
const minValidBodyLength = 300;
const bodyInvalidTagId = "#article-body .invalid-label";

let metadataInputText;
let metadataRemainingMaxLengthText;
let metadataInvalidTag;

const metadataInputTextId = "#article-metadata #article-metadata-text";
const metadataRemainingMaxLengthTextId = "#article-metadata .max-length-previewer";
const metadataInvalidTagId = "#article-metadata .invalid-label";

let isTitleValid = {valid: false};
let isBodySummaryValid = {valid: false};
let isBodyValid = {valid: false};
let isMetadataValid = {valid: false};


function textInputCheckIfValidCallback(elementLength, isValidLength, isValidBool, invalidTagElement) {
    if (elementLength >= isValidLength) {
        isValidBool.valid = true;

        invalidTagElement.style.visibility = "hidden";
    } else {
        isValidBool.valid = false;
    }
}

document.addEventListener("DOMContentLoaded", function() {
    titleInputText = document.querySelector(titleInputTextId);
    titleRemainingMaxLengthText = document.querySelector(titleRemainingMaxLengthTextId);
    titleInvalidTag = document.querySelector(titleInvalidTagId);

    titleInvalidTag.style.visibility = "hidden";

    titleInputText.addEventListener("input", function() {
        displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);
        textInputCheckIfValidCallback(titleInputText.value.length, 1, isTitleValid, titleInvalidTag);
    });

    displayTextLengthLeft(titleInputTextId, titleRemainingMaxLengthTextId, false);

    bodySummaryInputText = document.querySelector(bodySummaryInputTextId);
    bodySummaryRemainingMaxLengthText = document.querySelector(bodySummaryRemainingMaxLengthTextId);
    bodySummaryInvalidTag = document.querySelector(bodySummaryInvalidTagId);

    bodySummaryInvalidTag.style.visibility = "hidden";

    bodySummaryInputText.addEventListener("input", function() {
        displayTextLengthLeft(bodySummaryInputTextId, bodySummaryRemainingMaxLengthTextId);
        textInputCheckIfValidCallback(bodySummaryInputText.value.length, bodySummaryInputTextMinLength, isBodySummaryValid, bodySummaryInvalidTag);
    });

    bodySummaryInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(bodySummaryInputTextId, bodySummaryRemainingMaxLengthTextId);


    bodyInputText = document.querySelector(bodyInputTextId);
    bodyRemainingMaxLengthText = document.querySelector(bodyRemainingMaxLengthTextId);
    bodyInvalidTag = document.querySelector(bodyInvalidTagId);

    bodyInvalidTag.style.visibility = "hidden";

    bodyInputText.addEventListener("input", function() {
        displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);
        textInputCheckIfValidCallback(bodyInputText.value.length, minValidBodyLength, isBodyValid, bodyInvalidTag);
    });

    bodyInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(bodyInputTextId, bodyRemainingMaxLengthTextId);

    metadataInputText = document.querySelector(metadataInputTextId);
    metadataRemainingMaxLengthText = document.querySelector(metadataRemainingMaxLengthTextId);    
    metadataInvalidTag = document.querySelector(metadataInvalidTagId);

    metadataInvalidTag.style.visibility = "hidden";

    metadataInputText.addEventListener("input", function() {
        displayTextLengthLeft(metadataInputTextId, metadataRemainingMaxLengthTextId);
        textInputCheckIfValidCallback(metadataInputText.value.length, 1, isMetadataValid, metadataInvalidTag);
    })

    metadataInputText.addEventListener("input", autoResizeTextArea, false);

    displayTextLengthLeft(metadataInputTextId, metadataRemainingMaxLengthTextId);

    document.getElementById("publish-article").onclick = () => {
        createArticle();
    }
});

function displayTextLengthLeft(textBoxId, textLeftDisplayId, isTextField = true) {
    if (isTextField) {
        const bodyInputText = document.querySelector(textBoxId);
        const remainingMaxLengthText = document.querySelector(textLeftDisplayId);

        const remainingLettersAmount = bodyInputText.maxLength - bodyInputText.textLength;

        remainingMaxLengthText.innerHTML = "Remaining letters: " + remainingLettersAmount;

        if (remainingLettersAmount == 0) {
            remainingMaxLengthText.style.color = "red";
        } else {
            remainingMaxLengthText.style.color = "black";
        }
    } else {
        const bodyInputText = document.querySelector(textBoxId);
        const remainingMaxLengthText = document.querySelector(textLeftDisplayId);

        const remainingLettersAmount = bodyInputText.maxLength - bodyInputText.value.length;

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

    let formattedText;
    if (endTag) {
        formattedText = startTag + selectedText + endTag;
    } else { 
        formattedText = startTag + selectedText;
    }

    if (isHeader) {
        let remainingBodyText = textAreaText.substring(textAreaSelectionEnd);

        // Remove leading whitespace and newlines
        remainingBodyText = remainingBodyText.replace(/^\s+/, "");

        textArea.value = textAreaText.substring(0, textAreaSelectionStart) + formattedText + "\r\n\r\n" + remainingBodyText;
    } else {
        textArea.value = textAreaText.substring(0, textAreaSelectionStart) + formattedText + textAreaText.substring(textAreaSelectionEnd);
    }    


    // Reset the cursor position
    textArea.selectionStart = textAreaSelectionStart;
    textArea.selectionEnd = textAreaSelectionEnd;

    // Bring back focus on the text area so that the user can continue typing
    textArea.focus();

    // Trigger the input event to auto resize the field accordingly
    bodyInput.dispatchEvent(new Event("input"))

    console.log("Styled text");
}

async function createArticle() {
    if (isTitleValid.valid && isBodySummaryValid.valid && isBodyValid.valid && isMetadataValid.valid) {

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
    } else {
            console.log("Cannot create article, please input something into the fields!")

            if (!isTitleValid.valid) {
                titleInvalidTag.style.visibility = "visible"
            }
            if (!isBodySummaryValid.valid) {
                bodySummaryInvalidTag.style.visibility = "visible";
            }
            if (!isBodyValid.valid) {
                bodyInvalidTag.style.visibility = "visible";
            }
            if (!isMetadataValid.valid) {
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