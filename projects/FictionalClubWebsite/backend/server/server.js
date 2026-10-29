const http = require("http");

const server = http.createServer((req, res) => {
    console.log("got article save request the request");
});

const port = 3590;
const host = "localhost";

server.listen(port, host, () =>  {
    console.log(`Server running at http://${host}:${port}/`);
});