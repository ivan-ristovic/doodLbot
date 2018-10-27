// https://github.com/kittykatattack/learningPixi
// npm start
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
let GameState;

function setup() {
  console.log("All files loaded");
  cat = new Sprite(loader.resources["images/cat.png"].texture);
  // cat.visible = false;
  cat.x = 100
  cat.y = 200
  cat.vx = 0
  cat.vy = 0
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
    cat.vy = -5;
    cat.vx = 0;
  };
  up.release = () => {
    if (!down.isDown && cat.vx === 0) {
      cat.vy = 0;
    }
  };

  //left
  left.press = () => {
    cat.vx = -5;
    cat.vy = 0;
  };
  left.release = () => {
    if (!right.isDown && cat.vy === 0) {
      cat.vx = 0;
    }
  };

  //Right
  right.press = () => {
    cat.vx = 5;
    cat.vy = 0;
  };
  right.release = () => {
    if (!left.isDown && cat.vy === 0) {
      cat.vx = 0;
    }
  };

  //Down
  down.press = () => {
    cat.vy = 5;
    cat.vx = 0;
  };
  down.release = () => {
    if (!up.isDown && cat.vx === 0) {
      cat.vy = 0;
    }
  };

  let message = new PIXI.Text("Java is the best programming language.");
  app.stage.addChild(message);

  GameState = play
  // game loop
  app.ticker.add(delta => gameLoop(delta))
}

// delta is 1 if running at 100% performance
// creates frame-independent transformation
function gameLoop(delta){
  // console.log(delta)
  GameState(delta)
}

function play(delta){
  // console.log(delta)
  cat.x += cat.vx * delta
  cat.y += cat.vy * delta
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
    // console.log(event.keyCode)
    if (event.keyCode === key.code) {
      if (key.isUp && key.press) key.press();
      key.isDown = true;
      key.isUp = false;
      return;
    }
    //event.preventDefault();
  };

  //The `upHandler`
  key.upHandler = event => {
    if (event.keyCode === key.code) {
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
