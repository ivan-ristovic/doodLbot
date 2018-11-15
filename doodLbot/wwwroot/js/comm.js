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
    let toSend = code.elements;
    console.log(JSON.stringify(toSend, null, 2));
    connection.invoke("algorithmUpdated", JSON.stringify(toSend)).catch(function (err) {
        return console.error(err.toString());
    });
}

// initialize variables. This is the first server response.
function initClient(data) {
    let alg = data.algorithm;
    console.log("Initial handshake recieved.")
    updateCodeBlocks(alg);
    //console.log(JSON.stringify(alg, null, 2));
    ServerTickrate = data.tickRate;
    MulSpeedsWith = ServerTickrate / 60;
    MapWidth = data.mapWidth;
    MapHeight = data.mapHeight;
}

// starts up connection from clients side
function startConnection() {
    // callbacks for server pushing data to client
    connection.on("StateUpdate", onStateUpdate);
    connection.on("UpdateCodeBlocks", updateCodeBlocks);
    connection.on("InitClient", initClient);

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
        .invoke("TestingCallback")
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