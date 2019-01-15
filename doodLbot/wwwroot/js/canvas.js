"use strict";
// https://github.com/kittykatattack/learningPixi

// SERVER => CLIENT
function onStateUpdate(gameState) {
    if (WhatToRender != play) {
        // ignore this if handshake was not received
        return;
    }
    timesRecieved++;
    FramesSinceLastUpdate = 0;
    GAMESTATE = new GameState(gameState);
    CheckForNewHeroes();
    CheckForDeletedHeroes();
    updateHeroGear();
    updateHeroName();
    serverCounter.countTimesPerSecond(true);
}

var isReadyToPlay = false;
var FramesSinceLastUpdate = 0;
var ServerTickrate = null;
var MulSpeedsWith = null;
var MapWidth = null;
var MapHeight = null;
// sprites

let heroClasses = []

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
        innerBar.beginFill(0x111111);
        innerBar.drawRect(-2, -2, w + 4, h + 4);
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

class Hero extends Entity {
    constructor(nameGroup, id, heroGroup, healthBarGroup, pts = 0) {
        super()
        this.nameGroup = nameGroup;
        this.id = id
        this.heroGroup = heroGroup
        this.healthBarGroup = healthBarGroup
        this.pts = pts
    }


}

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

function tintFromDistance(x1, x2, y1, y2) {
    let a = x1 - x2;
    let b = y1 - y2;

    let c = Math.sqrt(a * a + b * b);
    let amount = 1 / (1 + c / 300);
    let red = amount * 255;
    let green = 1 - amount * 255;
    return (red << 16) + (green << 8);
}

function updateHeroGear() {
    for (let i = 0; i < GAMESTATE.heroes.length; i++) {
        // TODO: make this more robust:
        let heroToUpdateGear = getHeroById(GAMESTATE.heroes[i].id)
        if (GAMESTATE.heroes[i].gear.length > heroToUpdateGear.heroGroup.children.length - 1) {
            console.log(heroToUpdateGear.heroGroup, GAMESTATE.heroes[i].gear);
            heroToUpdateGear.heroGroup.addChildAt(hoverboard, 0);
        } else if (GAMESTATE.heroes[i].gear.length < heroToUpdateGear.heroGroup.children.length - 1) {
            heroToUpdateGear.heroGroup.removeChild(hoverboard);
        }
    }
}

function updateHeroName() {
    // TODO: do this only once, not on very thick
    for (let i = 0; i < GAMESTATE.heroes.length; i++) {
        if (GAMESTATE.heroes[i].name != null) {
            let heroToUpdateName = getHeroById(GAMESTATE.heroes[i].id)
            heroToUpdateName.nameGroup._text = GAMESTATE.heroes[i].name
            heroToUpdateName.nameGroup.style = new PIXI.TextStyle();
        }
    }
}

const halfPI = Math.PI / 2;

function updateProjectiles() {
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
        // TODO Prosledi HP
        app.stage.addChild(EnemyHps[EnemyHps.length - 1]);
    }

    //TODO: determine closest hero to attack
    let playerToAttack = getCurrentHero().heroGroup;

    let enemies = GAMESTATE.enemies;
    let speedMul = FramesSinceLastUpdate * MulSpeedsWith;
    for (let i = 0; i < enemies.length; i++) {
        let newx = enemies[i].x + speedMul * enemies[i].vx;
        let newy = enemies[i].y + speedMul * enemies[i].vy;
        EnemySprites[i].position.set(newx, newy);
        EnemySprites[i].visible = true;
        EnemySprites[i].tint = tintFromDistance(newx, playerToAttack.position.x, newy, playerToAttack.position.y);
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

function getCurrentHero() {
    return getHeroById(id)
}

function getHeroById(heroId) {
    for (let i = 0; i < heroClasses.length; i++) {
        if (heroClasses[i].id == heroId) {
            return heroClasses[i];
        }
    }

    return undefined
}

function updateHud() {
    let currentHero = getCurrentHero();
    if (currentHero === undefined) {
        $("#pts")[0].innerHTML = "0";
        return;
    }
    $("#pts")[0].innerHTML = currentHero.pts;
}

function CheckForNewHeroes() {
    for (let i = 0; i < GAMESTATE.heroes.length; i++) {
        let existingHero = getHeroById(GAMESTATE.heroes[i].id)
        if (existingHero === undefined) {
            let newHeroGroup = createNewHeroGroup(id);
            newHeroGroup.visible = true;

            let heroHealthBar = Entity.createHealthBar(100, 10, newHeroGroup);
            heroHealthBar.visible = true;

            let heroName = createNewHeroNameGroup(GAMESTATE.heroes[i].name);
            heroName.visible = true;

            app.stage.addChild(newHeroGroup);
            app.stage.addChild(heroHealthBar);
            app.stage.addChild(heroName);

            let newHeroId = GAMESTATE.heroes[i].id
            let newHero = new Hero(heroName, newHeroId, newHeroGroup, heroHealthBar)
            heroClasses.push(newHero)
        }
    }
}

function createNewHeroGroup(tintSeed) {
    let heroGroup = new PIXI.Container()
    let hero = new Sprite(loader.resources["images/hero.png"].texture);
    if (tintSeed != undefined) {
        hero.tint = Math.floor((Math.abs(Math.sin(tintSeed + 4) * 16777215)) % 16777215);
    }
    hero.scale.set(0.5, 0.5);
    // percentage of texture dimensions 0 to 1
    hero.anchor.set(0.5, 0.5);
    heroGroup.rotation = 0; // radians
    hoverboard = new Sprite(loader.resources["images/hoverboard.png"].texture);
    hoverboard.anchor.set(0.5, 0.5);
    hoverboard.scale.set(1.8, 1.5);
    hoverboard.rotation = Math.PI / 2;

    heroGroup.addChild(hero);
    return heroGroup;
}

function createNewHeroNameGroup(name) {
    let heroNameGroup = new PIXI.Text(name, { fontSize: 28 });
    return heroNameGroup;
}

function CheckForDeletedHeroes() {
    for (let i = 0; i < heroClasses.length; i++) {
        if (!heroExistsInGameState(heroClasses[i].id)) {
            // delete hero on frontend
            app.stage.removeChild(heroClasses[i].heroGroup);
            app.stage.removeChild(heroClasses[i].healthBarGroup);
            app.stage.removeChild(heroClasses[i].nameGroup);

            heroClasses.pop(heroClasses[i])
        }
    }
}

function heroExistsInGameState(heroId) {
    for (let i = 0; i < GAMESTATE.heroes.length; i++) {
        if (GAMESTATE.heroes[i].id == heroId)
            return true
    }
    return false
}

function updateHeroes(delta) {
    if (GAMESTATE.heroes == undefined) {
        return;
    }

    for (let i = 0; i < GAMESTATE.heroes.length; i++) {
        let hero = GAMESTATE.heroes[i];
        let speedMul = FramesSinceLastUpdate * MulSpeedsWith * delta;

        // if it our player, then only interpolate when keys are pressed, otherwise always
        // this prevents jittery camera movement when stopping
        let shouldInterpolate = i == id - 1 ? (up.isDown || down.isDown) : true;
        speedMul = shouldInterpolate ? speedMul : 0;
        let xplus = speedMul * hero.vx;
        let yplus = speedMul * hero.vy;
        let newx = hero.x;
        let newy = hero.y;
        if (hero.x + xplus < MapWidth && hero.x + xplus > 0)
            newx += xplus;

        if (hero.y + yplus < MapHeight && hero.y + yplus > 0)
            newy += yplus;

        let heroToUpdate = getHeroById(hero.id)

        heroToUpdate.heroGroup.position.set(newx, newy);
        heroToUpdate.heroGroup.rotation = hero.rotation;
        Entity.updateHealthBar(newx, newy, hero.hp, heroToUpdate.healthBarGroup);
        heroToUpdate.nameGroup.position.set(newx - 40, newy + 60);
        heroToUpdate.pts = hero.pts
    }
}
const centerWith = navigator.platform == "MacIntel" ? 4 : 2;
// centers world on the player
function moveMap() {
    app.stage.position.x = app.renderer.width / centerWith;
    app.stage.position.y = app.renderer.height / centerWith;

    let currentHero = getCurrentHero()
    if (currentHero == undefined) {
        console.log("current hero (with id " + id + ") is undef");
        return;
    }

    app.stage.pivot.x = currentHero.heroGroup.position.x;
    app.stage.pivot.y = currentHero.heroGroup.position.y;
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
    //tilingBackground.tileScale.x = 2;
    //tilingBackground.tileScale.y = 3 / 2;

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

    let heroGroup = createNewHeroGroup();
    heroGroup.visible = false;

    let heroHealthBar = Entity.createHealthBar(100, 10, heroGroup);
    heroHealthBar.visible = false;

    let heroName = createNewHeroNameGroup(myName);
    heroName.visible = false;

    app.stage.addChild(heroGroup);
    app.stage.addChild(heroHealthBar);
    app.stage.addChild(heroName);

    let newHero = new Hero(heroName, id, heroGroup, heroHealthBar)
    heroClasses.push(newHero)

    loadingDraw = $("#loadingWindow");
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

    for (let i = 0; i < heroClasses.length; i++) {
        heroClasses[i].heroGroup.visible = true;
        heroClasses[i].healthBarGroup.visible = true;
        heroClasses[i].nameGroup.visible = true;
    }

    console.log(loadingDraw);
    loadingDraw.css("top", "-100%");
    setMapSize(MapWidth, MapHeight);
    WhatToRender = play;
    updateShop();
}

function play(delta) {
    fpsCounter.countTimesPerSecond(true);
    updateHeroes(delta);
    updateHud();
    updateEnemies();
    updateProjectiles();
    moveMap();
    FramesSinceLastUpdate++;
    GAMESTATE.update(UPDATES_FOR_BACKEND);
    UPDATES_FOR_BACKEND = new UpdatesForBackend();
    // TODO send heartbeat every 5s
    sendHeartbeatToServer(id)
}

let type = "WebGL";
if (!PIXI.utils.isWebGLSupported()) {
    type = "canvas";
}

PIXI.utils.sayHello(type);
resize();

startConnection();