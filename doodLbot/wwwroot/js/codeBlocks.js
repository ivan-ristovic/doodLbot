"use strict"

// stores codeBlocks data as an object
let CodeBlocks = null;

function createBlockLayout(domBlockIdNum) {
    let div = $("<div />")
        .addClass("card")
        .addClass("codeBlock")

    let checkbox = $("<input />", { type: 'checkbox', id: domBlockIdNum }).
        addClass("isOnCheckbox");
    let label = $("<label />", { for: domBlockIdNum }).
        addClass("titleLabel");

    let title = $("<div />")
        .addClass("title")

    title.append(checkbox);
    title.append(label);

    div.append(title);
    div.append($("<hr/>"));

    return div;
}

function addBlockType(blockDiv, blockJson) {
    let type = blockJson.type;
    let isActive = blockJson.isActive;

    blockDiv.addClass(type);

    $($(blockDiv).find(".isOnCheckbox")[0]).val(isActive);
    $($(blockDiv).find(".titleLabel")[0]).text(type);

    return blockDiv;
}

function addBranchingData(blockDiv) {
    blockDiv.append($("<div />").addClass("branchingIf").addClass("dropPart"));
    blockDiv.append("<hr />");
    blockDiv.append($("<div />").addClass("branchingThen").addClass("dropPart"));
    blockDiv.append("<hr />");
    blockDiv.append($("<div />").addClass("branchingElse").addClass("dropPart"));

    return blockDiv;
}

function createBlockType(blockJson, domBlockIdNum) {
    var basicBlock = createBlockLayout(domBlockIdNum);
    var blockType = addBlockType(basicBlock, blockJson);

    if (blockJson.type == "BranchingElement") {
        blockType = addBranchingData(blockType);
    } else {
        blockType.append("<div />").addClass("dropPart");
    }

    return blockType;
}

function appendBlockAndChildren(blockJson, whereToAppend) {
    var blockDiv = createBlockType(blockJson, codeBlockIdNum);
    whereToAppend.append(blockDiv);
    codeBlockIdNum += 1;

    if (blockJson.type == "BranchingElement") {
        appendBlockAndChildren(blockJson.cond, blockDiv.find(".branchingIf"));
        for (var i = 0; i < blockJson.then.elements.length; i++) {
            appendBlockAndChildren(blockJson.then.elements[i], blockDiv.find(".branchingThen"));
        }
        for (var i = 0; i < blockJson.else.elements.length; i++) {
            appendBlockAndChildren(blockJson.else.elements[i], blockDiv.find(".branchingElse"));
        }
    }
}

var codeBlockIdNum = 0;

function updateCodeBlocks(data) {
    console.log("updating code blocks.");
    CodeBlocks = data;
    codeBlockIdNum = 0;

    $("#codeBlocks").innerHTML = "";
    for (var i = 0; i < data.elements.length; i++) {
        appendBlockAndChildren(data.elements[i], $("#codeBlocks"));
    }

    let x = generateCodeBlocksJson($("#codeBlocks"));
}

function generateCodeBlocksJson(container) {
    if (!container)
        return;

    let containerChildren = $(container).children();
    let arr = containerChildren.map(function (index) {
        let type = $(containerChildren[index]).find(".titleLabel")[0].innerHTML;

        var jsonData = {
            "type": type,
        }

        if (type === "BranchingElement") {
            jsonData.cond = generateCodeBlocksJson($(containerChildren[index]).find(".branchingIf")[0]);
            jsonData.then = generateCodeBlocksJson($(containerChildren[index]).find(".branchingThen")[0]);
            jsonData.else = generateCodeBlocksJson($(containerChildren[index]).find(".branchingElse")[0]);
        }

        if (!$(containerChildren[index]).is("#codeBlocks")) {
            let isChecked = $(containerChildren[index]).find("input")[0].value == "checked";
            jsonData.isActive = isChecked;
        }

        return jsonData;
    })
    let a = arr.toArray();

    if ($(container).hasClass("branchingIf")) {
        // branching if only has 1 child
        a = a[0];
    } else {
        a = { "elements": a };
        if ($(container).hasClass("branchingThen") ||
            $(container).hasClass("branchingElse")) {
            a.type = "CodeBlockElement";
        }
    }

    if (!$(container).is("#codeBlocks")) {
        let isChecked = $(container).find("input")[0].value == "checked";
        a.isActive = isChecked;
    }

    return a;
}