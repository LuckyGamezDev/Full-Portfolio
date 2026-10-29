const http = require('http'); 

const clientRequest = http.request({
    method: "POST",
    hostname: "localhost",  
    port: 3590,               
    path: "/"               
});


function saveNewArticle() {


    clientRequest.on("response", (response) => {
        response.on("data", (chunk) => {
            console.log(chunk.toString());
        });
    });

    clientRequest.end();
}