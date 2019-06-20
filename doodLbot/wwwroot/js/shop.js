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
        $(this).parent().parent().children(".elementTitle").text(item);
        $(this).parent().parent().children(".elementDetails").children(".elementCount").text("#" + count);
        $(this).parent().parent().children(".elementDetails").children(".elementCost").text(cost + "☉");
        type === "code" ? buyCodeServer(item) : buyGearServer(item);
        $("#pts")[0].innerHTML = hero.pts;
    }));
    container.append(($("<div />").addClass(type).text("-")).click(function () {
        if (count <= 0)
            return;
        count--;
        $(this).parent().parent().children(".elementTitle").text(item);
        $(this).parent().parent().children(".elementDetails").children(".elementCount").text("#" + count);
        $(this).parent().parent().children(".elementDetails").children(".elementCost").text(cost + "☉");
        type === "code" ? sellCodeServer(item) : sellGearServer(item);
        $("#pts")[0].innerHTML = GAMESTATE.heroes[id - 1].pts;
    }));
    if (type === "code")
        container.append(($("<div />").addClass(type).text(">")).click(function () {
            if (count <= 0)
                return;
            count--;
            $(this).parent().parent().children(".elementTitle").text(item);
            $(this).parent().parent().children(".elementDetails").children(".elementCount").text("#" + count);
            $(this).parent().parent().children(".elementDetails").children(".elementCost").text(cost + "☉");
            equipItemServer(item)
        }));;
    return container;
};

let instantiateShopItem = (element, count, type, cost) => {
    return $("<div />")
        .addClass("shopItem")
        .addClass(element.name)
        .append($("<div />").addClass("elementTitle").text(element.name))
        .append(($("<div />").addClass("elementDetails row"))
            .append($("<div />").addClass("elementCount col-6").text("#" + count))
            .append($("<div />").addClass("elementCost  col-6").text(cost + "☉")))
        .append(buySellEquip(element.name, count, type, element.cost));
};

function updateShop() {
    for (let i in codeInventory.items) {
        let block = codeInventory.items[i];
        $("#codeBlocksShop").append(instantiateShopItem(block.element, block.count, "code", block.element.cost));
    }

    for (let i in equipmentInventory.items) {
        let item = equipmentInventory.items[i];
        $("#gearShop").append(instantiateShopItem(item.element, item.count, "inventory", item.element.cost));
    }
}

$("#shopIcon").click(shopToggle);