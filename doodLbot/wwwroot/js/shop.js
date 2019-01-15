"use strict";

var codeInventory;
var equipmentInventory;

let shopToggle = () => {
    $("#shop").toggleClass("hiddenShop");
    $("#shopIcon").toggleClass("closed");
    event.preventDefault();
};

let buySellEquip = (item, count, type, cost) => {
    let container = $("<div />").addClass("buySellEquipContainer");
    container.append(($("<div />").addClass(type).text("+")).click(function () {
        let hero = GAMESTATE.heroes[id - 1];
        if (hero.pts < cost)
            return;
        count++;
        $(this).parent().parent().children(".elementTitle").text(item + " (" + count + ")");
        type === "code" ? buyCodeServer(item) : buyGearServer(item);
        $("#pts")[0].innerHTML = hero.pts;
    }));
    container.append(($("<div />").addClass(type).text("-")).click(function () {
        if (count <= 0)
            return;
        count--;
        $(this).parent().parent().children(".elementTitle").text(item + " (" + count + ")");
        type === "code" ? sellCodeServer(item) : sellGearServer(item);
        $("#pts")[0].innerHTML = GAMESTATE.heroes[id - 1].pts;
    }));
    if (type === "code")
        container.append(($("<div />").addClass(type).text(">")).click(function () {
            if (count <= 0)
                return;
            count--;
            $(this).parent().parent().children(".elementTitle").text(item + " (" + count + ")");
            equipItemServer(item)
        }));;
    return container;
};

let instantiateShopItem = (element, count, type) => {
    return $("<div />")
        .addClass("shopItem")
        .addClass(element.name)
        .append($("<div />").addClass("elementTitle").text(element.name + " (" + count + ")"))
        .append(buySellEquip(element.name, count, type, element.cost));
};

function updateShop() {
    for (let i in codeInventory.items) {
        let block = codeInventory.items[i];
        $("#codeBlocksShop").append(instantiateShopItem(block.element, block.count, "code"));
    }

    for (let i in equipmentInventory.items) {
        let item = equipmentInventory.items[i];
        $("#gearShop").append(instantiateShopItem(item.element, item.count, "inventory"));
    }
}

$("#shopIcon").click(shopToggle);