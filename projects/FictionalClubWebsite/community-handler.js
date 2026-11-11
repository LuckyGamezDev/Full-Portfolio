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
    // Logic for opening the article with the correct, according article information
    var articleWindow = window.open("", "_self");
    articleWindow.document.innerHTML = article.articleHTML;

    console.log("Opened article: " + article.articleHTML);
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