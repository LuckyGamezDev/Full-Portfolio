let communityArticlesDiv;
const communityArticlesDivId = "community-posts";

const communityArticleDivClassName = "community-post-preview";
const communityArticlesTitleClassName = "title";
const communityArticlesBodySummaryClassName = "body-summary"
const communityArticlesRedirButtonClassName = "article-redir-community";

async function initializeArticles() {
    console.log("Making request");
    
    let allArticlesArray;

    try {
        const response = await fetch("http://localhost:3590/articles")

        if (!response.ok) {
            throw new Error("A HTTP error occured whilst attempting to retrieve article data. Status: " + response.status);    
        }

        if (response.status === 204) {
            console.log("No articles to initialize.");

            return;
        }

        allArticlesArray = await response.json();

    } catch (error) {
        console.log("An error occured whilst attempting to retrieve article data: " + error);
    }

    console.log(allArticlesArray.length);

    for (let i = 0; i < allArticlesArray.length; i++) {
        let article = allArticlesArray[i];

        loadArticle(new Article(article["title"], article["body-summary"], article["body"], article["sources"], article["html"]));
    } 

    console.log("Intialized articles!");
}


document.addEventListener("DOMContentLoaded", function() {
    communityArticlesDiv = document.getElementById(communityArticlesDivId);

    initializeArticles();
});

function loadArticle(article) {
    let articleDiv = document.createElement("div");
    articleDiv.className = communityArticleDivClassName;

    let articleTitle = document.createElement("p")
    articleTitle.className = communityArticlesTitleClassName;
    articleTitle.innerHTML = article.title;

    let articleBodySummary = document.createElement("p")
    articleBodySummary.className = communityArticlesBodySummaryClassName;
    articleBodySummary.innerHTML = article.bodySummary;

    let articleRedirButton = document.createElement("button");
    articleRedirButton.className = communityArticlesRedirButtonClassName;
    articleRedirButton.innerHTML = "To Article";
    articleRedirButton.onclick = () => {
        openArticle(article);
    }

    articleDiv.appendChild(articleTitle);
    articleDiv.appendChild(articleBodySummary);
    articleDiv.appendChild(articleRedirButton);
    
    communityArticlesDiv.appendChild(articleDiv);

    console.log("Successfully loaded an article!");
}

function openArticle(article) {
    const parser = new DOMParser();
    const doc = parser.parseFromString(article.articleHTML, "text/html");

    const articleBodyDiv = doc.querySelector("#article-body");

    const regex = /^\s*#{1,3}.*?#\s*$/gm;

    const content = articleBodyDiv.innerHTML;
    const headings = [...content.matchAll(regex)];
    const sections = [];

    for (let i = 0; i < headings.length; i++) {
        const start = headings[i].index;
        const end = headings[i + 1]?.index ?? content.length;

        const section = content.substring(start, end).trim();

        const match = section.match(/^(#{1,3})(.*?)\1\s*([\s\S]*)$/);

        if (!match) continue;

        const [, hashes, heading, body] = match;

        sections.push({
            headingText: heading.trim(),
            level: hashes.length,
            body: body.trim()
        });
    }

    articleBodyDiv.innerHTML = ""; // Reset the innerHTML to be empty
    for (let i = 0; i < sections.length; i++) {
        const lineBreakElement = doc.createElement("br");

        const headerElement =
            sections[i].level === 1 ? doc.createElement("h1") :
            sections[i].level === 2 ? doc.createElement("h2") :
            sections[i].level === 3 ? doc.createElement("h3") :
            null;
            
        headerElement.innerHTML = sections[i].headingText; 
        const headerBodyTextElement = doc.createElement("p");
        headerBodyTextElement.innerHTML = sections[i].body;

        console.log(headerElement);

        articleBodyDiv.appendChild(headerElement);
        articleBodyDiv.appendChild(headerBodyTextElement);
        articleBodyDiv.appendChild(lineBreakElement);
    }

    console.log(articleBodyDiv);
    
    const fullArticle = doc.documentElement.outerHTML;

    console.log(fullArticle);
    
    const newWindow = window.open("", "_self");
    if (fullArticle) {
        newWindow.document.open();
        newWindow.document.write(fullArticle);
        newWindow.document.close();
    } else {
        console.error("An error occured during the opening of an article!");
    }

    console.log("Opened article");
}

class Article {
    constructor(title, bodySummary, body, sources, articleHTML = null) {
        this.title = title;
        this.bodySummary = bodySummary;
        this.body = body;
        this.sources = sources;
        this.articleHTML = articleHTML;
    }
}