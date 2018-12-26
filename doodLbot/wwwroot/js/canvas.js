"use strict";
// https://github.com/kittykatattack/learningPixi

// SERVER => CLIENT
function onStateUpdate(gameState) {
    if (WhatToRender != play) {
        // ignore this if handshake was not recieved
        return;
    }
    timesRecieved++;
    FramesSinceLastUpdate = 0;
    GAMESTATE = new GameState(gameState);
    // TODO: make this more robust:
    if (GAMESTATE.hero.gear.length > heroGroup.children.length - 1) {
        console.log(heroGroup, GAMESTATE.hero.gear);
        heroGroup.addChildAt(hoverboard, 0);
    }
    serverCounter.countTimesPerSecond(true);
}
var id = -1;
var isReadyToPlay = false;
var FramesSinceLastUpdate = 0;
var ServerTickrate = null;
var MulSpeedsWith = null;
var MapWidth = null;
var MapHeight = null;
// sprites

let heroGroup;
let heroHealthBar;
let hoverboard;
// currently is only assigned to play inside setup(),
// but can be assigned to something else if needed, eg. menu.
let WhatToRender;

let loadingDraw;
let enemyTexture;
let tilingBackground = null;
let EnemySprites = [];
let ProjectileSprites = [];
let EnemyHps = [];

let Counter = function (id) {
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
    // FIXME. bar widths are incorrect at first
    // because we are always sending w=100
    static createHealthBar(w, h, sprite) {
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
        let before = healthBar.outer.width;
        healthBar.outer.width = (hp / 100) * healthBar.w;
        //console.log("hp=", hp, before, healthBar.outer.width);
    }
}

class Hero extends Entity {}

class Enemy extends Entity {}

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

    getHeroes() {
        return this.heroes;
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
    let red = amount * 255;
    let green = 1 - amount * 255;
    return (red << 16) + (green << 8);
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
        EnemySprites[i].tint = calcTint(newx, heroGroup.position.x, newy, heroGroup.position.y);
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

function updateHud() {
    if (GAMESTATE.hero === undefined) {
        $("#pts")[0].innerHTML = "points: 0";
        return;
    }
    $("#pts")[0].innerHTML = "points: " + GAMESTATE.hero.pts;
}

function updateHero(delta) {
    if (GAMESTATE.hero !== undefined) {
        let hero = GAMESTATE.hero;
        let speedMul = FramesSinceLastUpdate * MulSpeedsWith * delta;
        let shouldInterpolate = up.isDown || down.isDown;
        speedMul = shouldInterpolate ? speedMul : 0;
        let xplus = speedMul * hero.vx;
        let yplus = speedMul * hero.vy;
        let newx = hero.x;
        let newy = hero.y;
        if (hero.x + xplus < MapWidth && hero.x + xplus > 0)
            newx += xplus;

        if (hero.y + yplus < MapHeight && hero.y + yplus > 0)
            newy += yplus;

        heroGroup.position.set(newx, newy);
        heroGroup.rotation = hero.rotation;
        Entity.updateHealthBar(newx, newy, hero.hp, heroHealthBar);
    }
}
const centerWith = navigator.platform == "MacIntel" ? 4 : 2;
// centers world on the player
function moveMap() {
    app.stage.position.x = app.renderer.width / centerWith;
    app.stage.position.y = app.renderer.height / centerWith;

    app.stage.pivot.x = heroGroup.position.x;
    app.stage.pivot.y = heroGroup.position.y;
}

let GAMESTATE = new GameState();

// holds information that backend needs for updating game-state
class UpdatesForBackend {
    constructor() {
        this.timeSinceLastSend = 0;
        this.keyPresses = [];
        this.actions = [];
        this.id = id;
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
app.renderer.backgroundColor = 0x110021;

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

// draws edges of the map
function drawBorder(width, height, thickness) {
    console.log("drawing border");
    var rect = new PIXI.Graphics();
    rect.lineStyle(thickness, 0x000000, 1);
    rect.drawRect(0, 0, width, height);
    app.stage.addChild(rect);
}

// sets borders and background tiling
function setMapSize() {
    drawBorder(MapWidth, MapHeight, 10);
    if (tilingBackground != null) {
        app.stage.removeChild(tilingBackground);
    }
    let canvasw = app.renderer.width;
    let canvash = app.renderer.height;

    let w = app.renderer.width + MapWidth;
    let h = app.renderer.height + MapHeight;
    tilingBackground = new PIXI.extras.TilingSprite(loader.resources["images/paper.jpg"].texture, w, h);
    tilingBackground.position.set(-canvasw / 2, -canvash / 2);
    tilingBackground.tileScale.x = 2;
    tilingBackground.tileScale.y = 3 / 2;

    app.stage.addChildAt(tilingBackground, 0);
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
        "images/projectile.png",
        "images/hoverboard.png"

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
    heroGroup = new PIXI.Container()
    let hero = new Sprite(loader.resources["images/hero.png"].texture);
    hero.scale.set(0.5, 0.5);
    // percentage of texture dimensions 0 to 1
    hero.anchor.set(0.5, 0.5);
    heroGroup.rotation = 0; // radians
    hoverboard = new Sprite(loader.resources["images/hoverboard.png"].texture);
    hoverboard.anchor.set(0.5, 0.5);
    hoverboard.scale.set(1.8, 1.5);
    hoverboard.rotation = Math.PI / 2;

    heroGroup.addChild(hero);
    heroHealthBar = Entity.createHealthBar(100, 10, heroGroup);

    let text = new PIXI.Text('Establishing connection to server...', {
        fontFamily: 'Arial',
        fontSize: 24,
        fill: 0x9999f0,
        align: 'center'
    });
    loadingDraw = text;
    loadingDraw.position.set(100, 100);
    app.stage.addChild(loadingDraw);
    app.stage.addChild(heroGroup);
    heroGroup.visible = false;
    app.stage.addChild(heroHealthBar);
    heroHealthBar.visible = false;
    WhatToRender = WaitingForHandshake;
    // game loop
    app.ticker.add(delta => gameLoop(delta));
}

// delta is 1 if running at 100% performance
// creates frame-independent transformation
function gameLoop(delta) {
    WhatToRender(delta);
}

function WaitingForHandshake() {
    // could write some text like 'connecting'
    if (!isReadyToPlay) {
        return;
    }
    heroGroup.visible = true;
    heroHealthBar.visible = true;
    loadingDraw.visible = false;
    setMapSize(MapWidth, MapHeight);
    WhatToRender = play;
    updateShop();
}

function play(delta) {
    fpsCounter.countTimesPerSecond(true);
    updateHero(delta);
    updateHud();
    updateEnemies();
    updateProjectiles();
    moveMap();
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