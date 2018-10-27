// https://github.com/kittykatattack/learningPixi

//Create a Pixi Application
let app = new PIXI.Application({width: 256, 
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
PIXI.loader
  .add(//[
    "images/cat.png",
    //"https://raw.githubusercontent.com/kittykatattack/learningPixi/master/examples/images/cat.png"
    //"images/imageTwo.png",
    //"images/imageThree.png"
  //]
  //, { crossOrigin: true}
  )
  .load(setup);

function setup() {
    let cat = new PIXI.Sprite(PIXI.loader.resources["images/cat.png"].texture);
    app.stage.addChild(cat);
}

//Add the canvas that Pixi automatically created for you to the HTML document

let type = "WebGL"
if(!PIXI.utils.isWebGLSupported()){
    type = "canvas"
}

PIXI.utils.sayHello(type)
