// https://github.com/kittykatattack/learningPixi

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.textContent = user + " says " + message;;
    document.getElementById("consoleDiv").appendChild(li);
    console.log("Recieved message!", user, message);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
// parent class for hero and enemies

class Entity {
  constructor(){
    this.x = 0;
    this.y = 0;
    this.vx = 0;
    this.vy = 0;
    this.hp = 100;
    this.rotation = 0;
    this.damage = 1;
  }
}
class Hero extends Entity {
  constructor(){
    super();
    this.gear = [];
    this.modules = [];
  }
}

class Enemy extends Entity {
  constructor(){
    super();
    this.type = "default";
  }
}

// holds all needed game-state, which will be updated by backend
class GameState {
  constructor(){
    this.hero = new Hero();
    this.enemies = [new Enemy()];
  }
  getEnemies(){
    return this.enemies;
  }

  getHero(){
    return this.hero;
  }

  // calls backend to update itself
  update(data) {
    //console.log(data.keyPresses);
    this.dummy();
  }

  // dummy placeholder instead of server update
  dummy(){
    let hero = GAMESTATE.getHero();
    hero.x += hero.vx;
    hero.y += hero.vy;
  }
}


let GAMESTATE = new GameState();

// holds information that backend needs for updating game-state
class UpdatesForBackend{
  constructor(){
    this.timeSinceLastSend = 0;
    this.keyPresses = [];
    this.actionsPerformed = [];
  }

  addKeyPress(key){
    this.keyPresses.push(key);
  }

  addKeyRelease(key){
    this.keyPresses.push(-key);
  }
}

let UPDATES_FOR_BACKEND = new UpdatesForBackend();

// using Aliases
let Application = PIXI.Application,
    loader = PIXI.loader,
    resources = PIXI.loader.resources,
    Sprite = PIXI.Sprite;
    TextureCache = PIXI.utils.TextureCache

// can use only one texture
let superFastSprites = new PIXI.particles.ParticleContainer(
  100, 
  {
  rotation: true,
  alphaAndtint: true,
  scale: true,
  uvs: false
});



//Create a Pixi Application
let app = new Application({width: 256, 
    height: 256,
    antialias: true,    
    transparent: false, 
    resolution: 1    
});
app.renderer.backgroundColor = 0x061639;

// make canvas fill the whole window
app.renderer.view.style.position = "absolute";
app.renderer.view.style.display = "block";
app.renderer.autoResize = true;
app.renderer.resize(window.innerWidth, window.innerHeight);
document.body.appendChild(app.view);

// The stage object is the root container for all the visible things in your scene. 
// Whatever you put inside the stage will be rendered on the canvas.
// app.stage

// images need to be added to texture cache for gpu 

// importing images from local filesystem is prohibited
// Instead, you need to run a local server to serve your files.

// spritesheet is an efficient way to store multiple sprites
loader
  .add([
    "images/cat.png",
    "images/paper.jpg"
    //"https://raw.githubusercontent.com/kittykatattack/learningPixi/master/examples/images/cat.png",
    //"images/imageTwo.png",
    //"images/imageThree.png"
  ]
  )
  .on("progress", loadProgressHandler)
  .load(setup);

function loadProgressHandler(loader, resource) {
  console.log("loading"); 
  //Display the file `url` currently being loaded
  console.log("loading: " + resource.url); 
  //Display the percentage of files currently loaded
  console.log("progress: " + loader.progress + "%"); 
}

// sprites
let cat;

// currently is only assigned to play inside setup(),
// but can be assigned to something else if needed, eg. menu.
let WhatToRender;


function setup() {
  console.log("All files loaded");
  cat = new Sprite(loader.resources["images/cat.png"].texture);
  // cat.visible = false;
  let hero = GAMESTATE.getHero();
  hero.x = 100
  hero.y = 200
  hero.vx = 0
  hero.vy = 0
  // cat.position.set(100, 200)
  cat.scale.set(0.5, 0.5)
  // percentage of texture dimensions 0 to 1
  cat.anchor.set(0.5, 0.5)
  cat.rotation = -1 // radians

  let paper = new Sprite(loader.resources["images/paper.jpg"].texture);
  paper.scale.set(4, 3)
  app.stage.addChild(paper);
  app.stage.addChild(cat);

  let left = keyboard(65),
      up = keyboard(87),
      right = keyboard(68),
      down = keyboard(83);

  // TODO: move these key setups in a seperate function
  //Up
  up.press = () => {
    hero.vy = -5;
  };
  up.release = () => {
    if (down.isDown){
      hero.vy = 5;
    } else {
      hero.vy = 0;
    }
  };

  //left
  left.press = () => {
    hero.vx = -5;
  };
  left.release = () => {
    if (right.isDown){
      hero.vx = 5;
    } else {
      hero.vx = 0;
    }
  };

  //Right
  right.press = () => {
    hero.vx = 5;
  };
  right.release = () => {
    if (left.isDown){
      hero.vx = -5;
    } else {
      hero.vx = 0;
    }
  };

  //Down
  down.press = () => {
    hero.vy = 5;
  };
  down.release = () => {
    if (up.isDown){
      hero.vy = -5;
    } else {
      hero.vy = 0;
    }
    };

    
    let testKey = keyboard(84);
    testKey.press = () => {
        connection.invoke("SendMessage", "user69", "thisIsTheMessage").catch(function (err) {
            return console.error(err.toString());
        });
    };

  let message = new PIXI.Text("Java is the best programming language.");
  app.stage.addChild(message);

    WhatToRender = play;
  // game loop
  app.ticker.add(delta => gameLoop(delta))
}

// delta is 1 if running at 100% performance
// creates frame-independent transformation
function gameLoop(delta){
  // console.log(delta)
  WhatToRender(delta)
}

function play(delta){
  // console.log(delta)
  let hero = GAMESTATE.getHero();
  cat.position.set(hero.x, hero.y);
  GAMESTATE.update(UPDATES_FOR_BACKEND);
  UPDATES_FOR_BACKEND = new UpdatesForBackend();
}
//Add the canvas that Pixi automatically created for you to the HTML document

let type = "WebGL"
if(!PIXI.utils.isWebGLSupported()){
    type = "canvas"
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
      UPDATES_FOR_BACKEND.addKeyPress(key.code);
      if (key.isUp && key.press) key.press();
      key.isDown = true;
      key.isUp = false;
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
      return;
    }
  };

  //Attach event listeners
  window.addEventListener(
    "keydown", key.downHandler.bind(key), false
  );
  window.addEventListener(
    "keyup", key.upHandler.bind(key), false
  );
  return key;
}

PIXI.utils.sayHello(type)

// use new Container() when grouping of sprites is needed
