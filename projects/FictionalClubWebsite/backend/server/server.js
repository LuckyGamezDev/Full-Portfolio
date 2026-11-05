const http = require("http");
const fs = require('node:fs');

const projectRootDirectory = "Z:/Programming/_programming-future-roadmap/_Github Repository/Full-Documented-Learning-Repository/projects/FictionalClubWebsite";

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

        let jsonFile = ""; // Why do you explicitely need to assign it as string? Why doesn't it automatically become one when assigning the chunk(s) to it?
        req.on("data", (chunk) => {
            jsonFile += chunk.toString();
        });


        req.on("end", () => {
            let randomFileNumbers = "";
            for (let i = 0; i < 8; i++) {
                randomFileNumbers += Math.floor(Math.random() * 10).toString();
            }

            console.log("Writing these file contents: ", jsonFile);

            try {
                fs.writeFile(projectRootDirectory + "/articles/user-made/article_" + randomFileNumbers + ".json", jsonFile, err => {
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

        res.writeHead(204);
        res.end("Got article!");
    }

    if (req.method === "GET" && req.url === "/articles") {
        console.log("Got get request");

        let allArticleFilePathsArray = await readAllArticleFilePaths();

        console.log("First article path: " + allArticleFilePathsArray[0]);

        let allArticleFileContentsArray = getAllDataFromDirectoryByFileNamesArray(allArticleFilePathsArray);

        if (allArticleFileContentsArray) {
        
            console.log("Ending response...");

            res.writeHead(200);
            res.end(JSON.stringify(allArticleFileContentsArray));

            console.log("sent file contents");
        } else {
            console.error("An error occured during the reading of all the articles!");

            res.writeHead(500);
            res.end();

            return;
        }
    }
});

function getAllDataFromDirectoryByFileNamesArray(filePathsArray) {
    console.log("File paths array length: " + filePathsArray.length);

    let allFileDataStringsArray = [];
    for (let i = 0; i < filePathsArray.length; i++) {
        allFileDataStringsArray.push(fs.readFileSync(filePathsArray[i], { encoding: "utf8" }));
    }

    console.log("Return value length: " + allFileDataStringsArray.length);

    return allFileDataStringsArray;
}

async function readAllArticleFilePaths() {
    console.log("Reading all the files...");

    let allArticleFileNamesArray = [];
    try {
        console.log(typeof(allArticleFileNamesArray));
        allArticleFileNamesArray.push(projectRootDirectory + "/articles/user-made/" + await fs.promises.readdir(projectRootDirectory + "/articles/user-made"));
    } catch (err) {
        console.error("ERROR: " + err);

        return null;
    }

    console.log("Read all the files...");
    console.log("First file path: " + allArticleFileNamesArray[0]);

    return allArticleFileNamesArray;
}

const port = 3590;
const host = "localhost";

server.listen(port, host, () =>  {
    console.log(`Server running at http://${host}:${port}/`);
});