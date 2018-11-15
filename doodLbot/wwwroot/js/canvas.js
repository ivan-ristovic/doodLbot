"use strict";
// https://github.com/kittykatattack/learningPixi

function onStateUpdate(gameState) {
    timesRecieved++;
    FramesSinceLastUpdate = 0;
    GAMESTATE = new GameState(gameState);
    serverCounter.countTimesPerSecond(true);
}

var FramesSinceLastUpdate = 0;
var ServerTickrate = null; 
var MulSpeedsWith = null;
var MapWidth = null;
var MapHeight = null;
// sprites
let cat;
let heroHealthBar;

// currently is only assigned to play inside setup(),
// but can be assigned to something else if needed, eg. menu.
let WhatToRender;

let enemyTexture;

let EnemySprites = [];
let ProjectileSprites = [];
let EnemyHps = [];

let Counter = function(id){
    var oldCountTime = performance.now();
    var frame = 0;
    var timeSum = 0;
    this.countTimesPerSecond = function (shouldPrint) {
        const now = performance.now();
        let diff = now - oldCountTime;
        oldCountTime = now;
        frame++;
        timeSum += diff;
        if (timeSum > 1000) {
            if (shouldPrint) {
                let isFps = id.includes("fps");
                let txt = isFps ? "fps" : "server ups";
                txt += "=" + (frame * 1000) / timeSum;
                document.querySelector(id).innerHTML = txt;
            }
            timeSum -= 1000;
            frame = 0;
            timeSum = 0;
        }
    }
}

let fpsCounter = new Counter("#fps");
let serverCounter = new Counter("#serverUps");

// parent class for hero and enemies
class Entity {
    static createHealthBar(w = 100, h = 10, sprite) {
        //Create the health bar
        let healthBar;
        healthBar = new PIXI.Container();
        healthBar.position.set(0, 0);

        // Bigger enemy -> bigger hp bar
        w *= Math.sqrt(sprite.width * sprite.height) / 100;

        //Create the black background rectangle
        let innerBar = new PIXI.Graphics();
        innerBar.beginFill(0x000000);
        innerBar.drawRect(0, 0, w, h);
        innerBar.endFill();
        healthBar.addChild(innerBar);

        //Create the front red rectangle
        let outerBar = new PIXI.Graphics();
        outerBar.beginFill(0xff3300);
        outerBar.drawRect(0, 0, w, h);
        outerBar.endFill();
        healthBar.addChild(outerBar);

        healthBar.outer = outerBar;
        healthBar.w = w;
        healthBar.h = h;
        healthBar.sprite = sprite;

        return healthBar;
    }

    static updateHealthBar(x, y, hp, healthBar) {
        healthBar.position.set(
            x - healthBar.width / 2,
            y - healthBar.sprite.height / 2 - 15
        );

        healthBar.outer.width = (hp / 100) * healthBar.w;
    }
}

class Hero extends Entity { }

class Enemy extends Entity { }

// holds all needed game-state, which will be updated by backend
class GameState {
    constructor(obj) {
        if (obj !== undefined) {
            let cast = Object.assign(this, obj);
            return cast;
        }
        console.log("WARNIG! GameState invalid obj");
    }

    getEnemies() {
        return this.enemies;
    }

    getHero() {
        return this.hero;
    }

    // calls backend to update itself
    update(data) {
        if (UPDATES_FOR_BACKEND.keyPresses.length === 0) {
            // then don't bother server
            return;
        }
        sendUpdateToServer(data);
    }
}

function calcTint(x1, x2, y1, y2) {
    let a = x1 - x2;
    let b = y1 - y2;

    let c = Math.sqrt(a * a + b * b);
    let amount = 1 / (1 + c / 300);
    let red =  amount * 255;
    let green = 1 - amount * 255;
    return (red << 16) + (green << 8) ;
}

const halfPI = Math.PI / 2;
function updateProjectiles() {
    // TODO - maybe it can be nicely merged with updateEnemies(),
    // the only prob are health bars.
    if (GAMESTATE.projectiles === undefined) return;
    let numProjs = GAMESTATE.projectiles.length;
    while (ProjectileSprites.length < numProjs) {
        var textr = new Sprite(loader.resources["images/projectile.png"].texture);
        textr.anchor.set(0.5, 0.5);
        ProjectileSprites.push(textr);
        app.stage.addChild(textr);
    }
    let projectiles = GAMESTATE.projectiles;
    let speedMul = FramesSinceLastUpdate * MulSpeedsWith;

    for (let i = 0; i < projectiles.length; i++) {
        let newx = projectiles[i].x + speedMul * projectiles[i].vx;
        let newy = projectiles[i].y + speedMul * projectiles[i].vy;
        ProjectileSprites[i].position.set(newx, newy);
        ProjectileSprites[i].visible = true;
        let ang = Math.atan2(projectiles[i].vy, projectiles[i].vx);
        ProjectileSprites[i].rotation = ang
    }

    for (let i = projectiles.length; i < ProjectileSprites.length; i++) {
        if (ProjectileSprites[i].visible === false) {
            break;
        }
        ProjectileSprites[i].visible = false;
    }
}

function updateEnemies() {
    if (GAMESTATE.enemies === undefined) return;

    let numEnemies = GAMESTATE.enemies.length;
    while (EnemySprites.length < numEnemies) {
        var enemyTexture = new Sprite(loader.resources["images/enemy.png"].texture);
        enemyTexture.anchor.set(0.5, 0.5);
        EnemySprites.push(enemyTexture);
        app.stage.addChild(enemyTexture);

        EnemyHps.push(Entity.createHealthBar(100, 10, enemyTexture));
        app.stage.addChild(EnemyHps[EnemyHps.length - 1]);
    }

    let enemies = GAMESTATE.enemies;
    let speedMul = FramesSinceLastUpdate * MulSpeedsWith;
    for (let i = 0; i < enemies.length; i++) {
        let newx = enemies[i].x + speedMul * enemies[i].vx;
        let newy = enemies[i].y + speedMul * enemies[i].vy;
        EnemySprites[i].position.set(newx, newy);
        EnemySprites[i].visible = true;
        EnemySprites[i].tint = calcTint(newx, cat.position.x, newy, cat.position.y);
        EnemyHps[i].visible = true;
        Entity.updateHealthBar(newx, newy, enemies[i].hp, EnemyHps[i]);
    }

    // if there are now less enemies than there are sprites, then don't draw them and don't draw their hp bar
    for (let i = enemies.length; i < EnemySprites.length; i++) {
        if (EnemySprites[i].visible === false) {
            break;
        }
        EnemySprites[i].visible = false;
        EnemyHps[i].visible = false;
    }
}

let GAMESTATE = new GameState();

// holds information that backend needs for updating game-state
class UpdatesForBackend {
    constructor() {
        this.timeSinceLastSend = 0;
        this.keyPresses = [];
        this.actions = [];
    }

    addKeyPress(key) {
        this.keyPresses.push(key);
    }

    addKeyRelease(key) {
        this.keyPresses.push(-key);
    }
}

let UPDATES_FOR_BACKEND = new UpdatesForBackend();

// using Aliases
let Application = PIXI.Application,
    loader = PIXI.loader,
    resources = PIXI.loader.resources,
    Sprite = PIXI.Sprite,
    TextureCache = PIXI.utils.TextureCache;

// can use only one texture
let superFastSprites = new PIXI.particles.ParticleContainer(100, {
    rotation: true,
    alphaAndtint: true,
    scale: true,
    uvs: false
});

//Create a Pixi Application
let app = new Application({
    width: 256,
    height: 256,
    antialias: true,
    transparent: false,
    resolution: devicePixelRatio
});
app.renderer.backgroundColor = 0x061639;

// make canvas fill the whole div
app.renderer.view.style.display = "block";
app.renderer.autoResize = true;
document.querySelector("#frame").appendChild(app.view);
window.addEventListener("resize", resize);

// Resize function window
function resize() {
    // Get the p
    const parent = app.view.parentNode;

    // Resize the renderer
    app.renderer.resize(parent.clientWidth, parent.clientHeight);
}

// The stage object is the root container for all the visible things in your scene.
// Whatever you put inside the stage will be rendered on the canvas.
// app.stage
loader
    .add([
        "images/hero.png",
        "images/paper.jpg",
        "images/particle.png",
        "images/enemy.png",
        "images/projectile.png"
    ])
    .on("progress", loadProgressHandler)
    .load(setup);

function loadProgressHandler(loader, resource) {
    console.log("loading");
    console.log("loading: " + resource.url);
    console.log("progress: " + loader.progress + "%");
}

function setup() {
    console.log("All files loaded");
    cat = new Sprite(loader.resources["images/hero.png"].texture);
    cat.scale.set(0.5, 0.5);
    // percentage of texture dimensions 0 to 1
    cat.anchor.set(0.5, 0.5);
    cat.rotation = 0; // radians

    heroHealthBar = Entity.createHealthBar(100, 10, cat);

    let paper = new Sprite(loader.resources["images/paper.jpg"].texture);
    paper.scale.set(4, 3);
    app.stage.addChild(paper);
    app.stage.addChild(cat);
    app.stage.addChild(heroHealthBar);

    WhatToRender = play;
    // game loop
    app.ticker.add(delta => gameLoop(delta));
}

// delta is 1 if running at 100% performance
// creates frame-independent transformation
function gameLoop(delta) {
    WhatToRender(delta);
}

function play(delta) {
    if (GAMESTATE.hero !== undefined) {
        let hero = GAMESTATE.hero;
        let speedMul = FramesSinceLastUpdate * MulSpeedsWith;
        let newx = hero.x + speedMul * hero.vx;
        let newy = hero.y + speedMul * hero.vy;

        cat.position.set(newx, newy);
        cat.rotation = hero.rotation;
        Entity.updateHealthBar(newx, newy, hero.hp, heroHealthBar);
    }
    fpsCounter.countTimesPerSecond(true);

    updateEnemies();
    updateProjectiles();
    FramesSinceLastUpdate++;
    GAMESTATE.update(UPDATES_FOR_BACKEND);
    UPDATES_FOR_BACKEND = new UpdatesForBackend();
}

let type = "WebGL";
if (!PIXI.utils.isWebGLSupported()) {
    type = "canvas";
}

PIXI.utils.sayHello(type);
resize();

startConnection();