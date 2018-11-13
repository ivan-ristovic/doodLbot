"use strict"

let connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

let consoleKeyFunc = () => {
    $("#consoleDiv").toggleClass("hiddenConsole");
    event.preventDefault();
}

function sendUpdateToServer(update) {
    connection.invoke("updateGameState", update).catch(function (err) {
        return console.error(err.toString());
    });
}

function sendCodeUpdateToServer(code) {
    connection.invoke("algorithmUpdated", code).catch(function (err) {
        return console.error(err.toString());
    });
}

function initClient(data) {
    let alg = data.algorithm;
    console.log("Initial handshake recieved.")
    updateCodeBlocks(alg);
    // TODO, set all other data here,
    // example: map with/height
}

function startConnection() {
    // callbacks for server pushing data to client
    connection.on("StateUpdate", onStateUpdate);
    connection.on("UpdateCodeBlocks", updateCodeBlocks);
    connection.on("InitClient", initClient);

    connection.on("ReceiveMessage", function (user, message) {
        var li = document.createElement("li");
        li.innerHTML = user + " says " + message;
        document.querySelector("#consoleDiv ul").appendChild(li);
        console.log("Recieved message!", user, message);
    });

    connection.start().then(function () {
        console.log('connection started');
        connection.invoke("ClientIsReady").catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

// listens for the given keyCode
function keyboard(keyCode) {
    let key = {};
    key.code = keyCode;
    key.isDown = false;
    key.isUp = true;
    key.press = undefined;
    key.release = undefined;
    //The `downHandler`
    key.downHandler = event => {
        if (event.keyCode === key.code) {
            //console.log("pressed", key.code);
            UPDATES_FOR_BACKEND.addKeyPress(key.code);
            if (key.isUp && key.press) key.press();
            key.isDown = true;
            key.isUp = false;
            event.preventDefault();
            return;
        }
    };

    //The `upHandler`
    key.upHandler = event => {
        if (event.keyCode === key.code) {
            UPDATES_FOR_BACKEND.addKeyRelease(key.code);
            if (key.isDown && key.release) key.release();
            key.isDown = false;
            key.isUp = true;
            event.preventDefault();
            return;
        }
    };

    //Attach event listeners
    window.addEventListener("keydown", key.downHandler.bind(key), false);
    window.addEventListener("keyup", key.upHandler.bind(key), false);
    return key;
}

let testKeyFunc = () => {
    connection
        .invoke("SendMessage", "user69", "thisIsTheMessage")
        .catch(function (err) {
            return console.error(err.toString());
        });
};

var timesRecieved = 0;
let left = keyboard(65),
    up = keyboard(87),
    right = keyboard(68),
    down = keyboard(83),
    space = keyboard(32);

let testKey = keyboard(84/*t*/);
testKey.press = testKeyFunc;

let consoleKey = keyboard(192/*~*/);
consoleKey.press = consoleKeyFunc;