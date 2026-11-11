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

var sourcesInputText;
var sourcesRemainingMaxLengthText;
var sourcesInvalidTag;

const sourcesInputTextId = "#article-sources #article-sources-text";
const sourcesRemainingMaxLengthTextId = "#article-sources .max-length-previewer";
const sourcesInvalidTagId = "#article-sources .invalid-label";

var isTitleValid = false;
var isBodySummaryValid = false;
var isBodyValid = false;
var isSourceValid = false;


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

async function createArticle() {
    if (isTitleValid && isBodySummaryValid && isBodyValid && isSourceValid) {
        var articleTitle = titleInputText.value;
        var articleBodySummary = bodySummaryInputText.value;
        var articleBody = bodyInputText.value;
        var articleSources = sourcesInputText.value;

        var createdArticle = new Article(articleTitle, articleBodySummary, articleBody, articleSources);
        console.log("Variable value: " + articleBodySummary, " The saved class value: " + createdArticle.bodySummary);
        var jsonStringArticle = createdArticle.convertToJson();

        console.log("Sending this article: " + jsonStringArticle);

        try {
            fetch("http://localhost:3590/articles", {
                method: "POST",
                body: jsonStringArticle,
                headers: {"Content-Type": "application/json"}
            }) .then((response) => console.log(response));
        } catch (err) {
            console.error("ERR: " + err);
        } 

        console.log("created article!")

        //window.open("community.html", "_top");

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
        if (!isSourceValid) {
            sourcesInvalidTag.style.visibility = "visible";
        }
    }
}

class Article {
    constructor(title, bodySummary, body, sources) {
        this.title = title;
        this.bodySummary = bodySummary,
        this.body = body;
        this.sources = sources;
    }

    convertToJson() {
        const jsonObject = {
            "title": this.title,
            "body-summary": this.bodySummary,
            "body": this.body,
            "sources": this.sources
        };

        console.log(jsonObject["body"]);

        return JSON.stringify(jsonObject);
    }
}