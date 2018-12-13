"use strict";

var codeInventory;
var equipmentInventory;

let shopToggle = () => {
    $("#shop").toggleClass("hiddenShop");
    $("#shopIcon").toggleClass("closed");
    event.preventDefault();
}



let instantiateShopItem = element => {
    return $("<div />")
        .addClass("shopItem")
        .addClass(element.name)
        .append(element.name)

}

function updateShop() {
    for (let i in codeInventory.items) {
        let block = codeInventory.items[i]
        $("#codeBlocksShop").append(instantiateShopItem(block.element));
    }

    for (let i in equipmentInventory.items) {
        let item = equipmentInventory.items[i]
        $("#gearShop").append(instantiateShopItem(item.element));
    }
}

$("#shopIcon").click(shopToggle);