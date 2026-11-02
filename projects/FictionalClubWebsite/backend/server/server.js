const http = require("http");
const fs = require('node:fs');

const projectRootDirectory = "Z:/Programming/_programming-future-roadmap/_Github Repository/Full-Documented-Learning-Repository/projects/FictionalClubWebsite";

const server = http.createServer((req, res) => {
    res.setHeader("Access-Control-Allow-Origin", "http://192.168.68.128:9999");
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

                res.end("An error occured whilst saving the file!");

                return;
            }
        });

        res.end("Got article!")
    }
});

const port = 3590;
const host = "localhost";

server.listen(port, host, () =>  {
    console.log(`Server running at http://${host}:${port}/`);
});