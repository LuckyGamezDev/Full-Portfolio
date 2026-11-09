const http = require("http");
const fs = require('node:fs');

const projectRootDirectory = "Z:/Programming/_programming-future-roadmap/_Github Repository/Full-Documented-Learning-Repository/projects/FictionalClubWebsite";
const serverDataInnerDirectory = "/backend/server/data/";

const server = http.createServer(async (req, res) => {
    res.setHeader("Access-Control-Allow-Origin", "*");
    res.setHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
    res.setHeader("Access-Control-Allow-Headers", "Content-Type");

    if (req.method === "OPTIONS" && req.url === "/articles") {
        console.log("got the article save OPTIONS request.");

        res.writeHead(204);
        res.end();

        return;
    }

    if (req.method === "POST" && req.url === "/articles") {
        console.log("got the article");

        var jsonFile = "";
        req.on("data", (chunk) => {
            jsonFile += chunk.toString();
        });

        
        var fileId = "";
        for (let i = 0; i < 8; i++) {
            fileId += Math.floor(Math.random() * 10).toString();
        }

        req.on("end", () => {

            // Add the ID to the json file itself to be able to fetch it
            var jsonFileDict = JSON.parse(jsonFile);
            jsonFileDict["id"] = fileId;
            jsonFile = JSON.stringify(jsonFileDict);

            console.log("Writing these file contents: ", jsonFile);

            try {
                fs.writeFile(projectRootDirectory + serverDataInnerDirectory + "articles/article_" + fileId + ".json", jsonFile, err => {
                    if (err) {
                        console.error("ERROR: " + err);
                    } else {
                        console.log("Succesfully wrote file!");
                    }
                });
            } catch (err) {
                console.log("ERROR: " + err);

                res.writeHead(500);
                res.end("An error occured whilst saving the file!");

                return;
            }
        });

        createHTMLFromJson(JSON.parse(jsonFile), fileId);

        res.writeHead(204);
        res.end("Got article!");
    }

    if (req.method === "GET" && req.url === "/articles") {
        console.log("Got get request");

        var allArticleFilePathsArray = await readAllArticleFilePaths();

        if (allArticleFilePathsArray == null) {
            console.log("is null: " + allArticleFilePathsArray);
        }

        if (!allArticleFilePathsArray) {
            res.writeHead(204)
            res.end("No articles found.");

            return;
        }

        console.log("First article path: " + allArticleFilePathsArray[0]);

        var allArticleFileContentsArray = getAllDataFromDirectoryByFileNamesArray(allArticleFilePathsArray);

        if (allArticleFileContentsArray) {


            const completeArticlesArray = allArticleFileContentsArray.map(article => {
                const html = fs.readFileSync(projectRootDirectory + serverDataInnerDirectory + "article_HTMLs/article_" + article["id"], "utf-8");

                return {
                    ...article,
                    html: html
                }
            })
        
            console.log("Ending response...");

            console.log("The json file being sent: " + JSON.stringify(completeArticlesArray));

            res.writeHead(200);
            res.end(JSON.stringify(completeArticlesArray));

            console.log("sent file contents");
        } else {
            if (!allArticleFileContentsArray) {
                console.warn("Article metadata array was empty.");
            }
            
            if (!allArticleFileContentsArray && !allArticleHTMLFileContentsArray) {
                console.log("Both the article meta data and HTML are null.");
            }

            res.writeHead(500);
            res.end();

            return;
        }
    }
});

function getAllDataFromDirectoryByFileNamesArray(filePathsArray) {
    console.log("File paths array length: " + filePathsArray.length);
    console.log("File paths array content: " + filePathsArray[0]);

    var allFileDataStringsArray = [];
    try {
        for (let i = 0; i < filePathsArray.length; i++) {
            var file = fs.readFileSync(filePathsArray[i], { encoding: "utf8" });

            if (file) {
                allFileDataStringsArray.push(file);
            }
        }
    } catch (err) {
        console.error("ERROR: " + err);
    }

    console.log("Return value length: " + allFileDataStringsArray.length);

    return allFileDataStringsArray;
}

async function readAllArticleFilePaths() {
    console.log("Reading all the files...");

    var allArticlesFullPathsArray;
    try {
        let articlesArray = await fs.promises.readdir(projectRootDirectory + serverDataInnerDirectory + "articles/");

        if (articlesArray.length === 0) {
            console.log("No Articles.");

            return null;
        }

        allArticlesFullPathsArray = articlesArray.map(fileName => {
            return projectRootDirectory + serverDataInnerDirectory + "articles/" + fileName;
        });
    } catch (err) {
        console.error("ERROR: " + err);

        return null;
    }

    console.log("Read all the files...");
    console.log("First file path: " + allArticlesFullPathsArray[0]);

    return allArticlesFullPathsArray;
}

function createHTMLFromJson(jsonFile, fileId) {
    var articleTitle = jsonFile["title"];
    var articleBody = jsonFile["body"];
    var articleSources = jsonFile["sources"];

    console.log("value of json file: " + jsonFile, jsonFile["title"], jsonFile["body"]);

    var articleHTMLString = 
    `
<!DOCTYPE html>

<html>
    <link rel="stylesheet" href="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\navigation-bar.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">

    <style>
        body {
            background-color: rgba(195, 200, 229, 0.723);
        }

        #article-body {
            background-color: rgba(42, 67, 93, 0.526);

            border-color: rgb(37, 41, 59);
            border-radius: 5px;
            border-style: solid;

            width: fit-content;
            padding: 15px;

            font-size: 20px;
        }

        #article-title {
            background-color: rgb(43, 61, 226);

            width: fit-content;

            text-align: center;

            border-radius: 5px;
            border-color: rgb(24, 40, 121);
            border-style: dashed;

            padding: 5px;
        }

        .article-sub-header {
            text-decoration: wavy;

            font-family: fantasy;
            font-size: 22px;
        }

        footer {
            background-color: rgb(31, 107, 182);

            width: fit-content;

            padding: 1px 10px;

            border-color: rgb(26, 67, 120);
            border-style: solid;
            border-radius: 3px;
        }
    </style>

    <script src="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\navigation-bar-script.js"></script>
    <body>
        <!-- # Top (persistent) navigation bar -->
        <div class="top-menu-nav">
            <a href="Z:\Programming\_programming-future-roadmap\_Github Repository\Full-Documented-Learning-Repository\projects\FictionalClubWebsite\index.html" id="home-title">The Gaming Buddies</a>
            
            <div id="top-menu-links">
                <a class="link-button" href="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\index.html">Home</a>
                <a class="link-button" href="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\join-us.html">Join Us</a>
                <a class="link-button" href="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\community.html">Our Community</a>
                <a class="link-button" href="Z:\\Programming\\_programming-future-roadmap\\_Github Repository\\Full-Documented-Learning-Repository\\projects\\FictionalClubWebsite\\about.html">About</a>
            </div>

            <a href="javascript:void(0);" class="icon" onclick="toggleNavigationMenu()">
                <i class="fa fa-bars"></i>
            </a>
        </div>

        <h1 id="article-title">${articleTitle}</h1>

        <p id="article-body">
            ${articleBody}
        </p>

        <footer>
            <p id="author">Placeholder</p>
            <p id="publish-date">Placeholder <span id="publish-date" style="font-style: italic;">placeholder</span></p>
        </footer>
    </body>
</html>
    `

    try {
        fs.writeFile(projectRootDirectory + serverDataInnerDirectory + "article_HTMLs/article_" + fileId + ".html", articleHTMLString, err => {
            if (err) {
                console.error("ERROR: " + err);
            } else {
                console.log("Succesfully wrote HTML file!");
            }
        });
    } catch (err) {
        console.log("ERROR: " + err);

        return;
    }
}

const port = 3590;
const host = "localhost";

server.listen(port, host, () =>  {
    console.log(`Server running at http://${host}:${port}/`);
});