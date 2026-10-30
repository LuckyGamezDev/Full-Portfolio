const http = require("http");

const server = http.createServer((req, res) => {
    res.setHeader("Access-Control-Allow-Origin", "http://localhost:9999");
    res.setHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
    res.setHeader("Access-Control-Allow-Headers", "Content-Type");

    if (req.method === "OPTIONS") {
        res.writeHead(204);
        res.end();

        return;
    }

    if (req.method === "POST") {
        console.log("got the article save request.");

        req.on("data", (chunk) => {
            console.log(chunk.toString());
        });

        res.end("Got article!")
    }
});

const port = 3590;
const host = "localhost";

server.listen(port, host, () =>  {
    console.log(`Server running at http://${host}:${port}/`);
});