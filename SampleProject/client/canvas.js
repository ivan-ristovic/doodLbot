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

//Add the canvas that Pixi automatically created for you to the HTML document
document.body.appendChild(app.view);

let type = "WebGL"
if(!PIXI.utils.isWebGLSupported()){
    type = "canvas"
}

PIXI.utils.sayHello(type)
