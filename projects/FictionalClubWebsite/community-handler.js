let communityArticlesDiv;
const communityArticlesDivId = "community-posts";

const communityArticleDivClassName = "community-post-preview";
const communityArticlesTitleClassName = "title";
const communityArticlesBodyClassName = "body";
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
    console.log(typeof(allArticlesArray));

    for (let i = 0; i < allArticlesArray.length; i++) {
        let article = JSON.parse(allArticlesArray[i]);
        loadArticle(new Article(article["title"], article["body"], article["sources"]));
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

    let articleBody = document.createElement("p")
    articleBody.className = communityArticlesBodyClassName;
    articleBody.innerHTML = article.body;

    let articleRedirButton = document.createElement("button");
    articleRedirButton.className = communityArticlesRedirButtonClassName;
    articleRedirButton.innerHTML = "To Article";
    articleRedirButton.onclick = () => {
        openArticle(article);
    }

    articleDiv.appendChild(articleTitle);
    articleDiv.appendChild(articleBody);
    articleDiv.appendChild(articleRedirButton);
    
    communityArticlesDiv.appendChild(articleDiv);

    console.log("Successfully loaded an article!");
}

function openArticle(article) {
    // Logic for opening the article with the correct, according article information
    window.open("articles/article-viewer.html", "_top");
}

class Article {
    constructor(title, body, sources) {
        this.title = title;
        this.body = body;
        this.sources = sources;
    }
}