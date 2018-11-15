"use strict"

// stores codeBlocks data as an object
let CodeBlocks = null;

$(document).ready(function () {
    $("#compile").click(function () {
        let json = generateCodeBlocksJson($("#codeBlocks"));
        console.log("sending alg");
        sendCodeUpdateToServer(json);
    });
});

function createBlockLayout(domBlockIdNum) {
    let div = $("<div />")
        .addClass("card")
        .addClass("codeBlock")

    let checkbox = $("<input />", {
        type: 'checkbox',
        id: domBlockIdNum
    }).
    addClass("isOnCheckbox");
    let label = $("<label />", {
        for: domBlockIdNum
    }).
    addClass("titleLabel");

    let title = $("<div />")
        .addClass("title")

    let dragDiv = $("<div/>").addClass("dragMe").attr("draggable", "true");

    title.append(checkbox);
    title.append(label);
    title.append(dragDiv)

    div.append(title);
    div.append($("<hr/>"));

    return div;
}

function addBlockType(blockDiv, blockJson) {
    let type = blockJson.type;
    let isActive = blockJson.isActive;

    blockDiv.addClass(type);

    $($(blockDiv).find(".isOnCheckbox")[0]).prop('checked', isActive);
    $($(blockDiv).find(".titleLabel")[0]).text(type);

    return blockDiv;
}

function addBranchingData(blockDiv) {
    blockDiv.append($("<div />").addClass("branchingIf"));
    blockDiv.append("<hr />");
    blockDiv.append($("<div />").addClass("branchingThen"));
    blockDiv.append("<hr />");
    blockDiv.append($("<div />").addClass("branchingElse"));

    return blockDiv;
}

function createBlockType(blockJson, domBlockIdNum, drop = false) {
    var basicBlock = createBlockLayout(domBlockIdNum, drop);
    var blockType = addBlockType(basicBlock, blockJson);

    if (blockJson.type == "BranchingElement") {
        blockType = addBranchingData(blockType);
    }

    return blockType;
}

function appendBlockAndChildren(blockJson, whereToAppend, drop = false) {
    var blockDiv = createBlockType(blockJson, codeBlockIdNum, drop);
    whereToAppend.append(blockDiv);
    if (drop)
        whereToAppend.append($("<div/>").addClass("dropPart"));
    codeBlockIdNum += 1;

    if (blockJson.type == "BranchingElement") {
        let branchingIf = blockDiv.find(".branchingIf")
        branchingIf.append($("<div/>").addClass("dropPart"));
        appendBlockAndChildren(blockJson.cond, branchingIf, true);

        let branchingThen = blockDiv.find(".branchingThen")
        branchingThen.append($("<div/>").addClass("dropPart"));
        for (var i = 0; i < blockJson.then.elements.length; i++) {
            appendBlockAndChildren(blockJson.then.elements[i], branchingThen, true);
        }

        let branchingElse = blockDiv.find(".branchingElse")
        branchingElse.append($("<div/>").addClass("dropPart"));
        for (var i = 0; i < blockJson.else.elements.length; i++) {
            appendBlockAndChildren(blockJson.else.elements[i], branchingElse, true);
        }
    }
}

var codeBlockIdNum = 0;

function updateCodeBlocks(data) {
    console.log("updating code blocks.");
    CodeBlocks = data;
    codeBlockIdNum = 0;

    $("#codeBlocks").innerHTML = "";

    $("#codeBlocks").append($("<div/>").addClass("dropPart"));
    for (var i = 0; i < data.elements.length; i++) {
        appendBlockAndChildren(data.elements[i], $("#codeBlocks"));
        $("#codeBlocks").append($("<div/>").addClass("dropPart"));
    }

    let x = generateCodeBlocksJson($("#codeBlocks"));

    $(".dragMe").each(function () {
        this.addEventListener('dragstart', dragStart);
        this.addEventListener('dragend', dragEnd);
    });

    // $(".dragMe")[0].addEventListener('dragstart', dragStart);
    // $(".dragMe")[0].addEventListener('dragend', dragEnd);

    $(".dropPart").each(function () {
        this.addEventListener('dragover', dragOver);
        this.addEventListener('dragenter', dragEnter);
        this.addEventListener('dragleave', dragLeave);
        this.addEventListener('drop', dragDrop);
    });
}

function generateCodeBlocksJson(container) {
    if (!container)
        return;

    let containerChildren = $(container).children(".codeBlock");
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
            let isChecked = $(containerChildren[index]).find("input")[0].checked;
            jsonData.isActive = isChecked;
        }

        return jsonData;
    })
    let a = arr.toArray();

    if ($(container).hasClass("branchingIf")) {
        // branching if only has 1 child
        a = a[0];
    } else {
        a = {
            "elements": a
        };
        if ($(container).hasClass("branchingThen") ||
            $(container).hasClass("branchingElse")) {
            a.type = "CodeBlockElement";
        }
    }

    if (!$(container).is("#codeBlocks")) {
        let isChecked = $(container).find("input")[0].checked;
        a.isActive = isChecked;
    }

    return a;
}

var draggingElement;

function dragStart() {
    draggingElement = $(this).parent().parent();
    $(".dropPart").each(function () {
        $(this).addClass("active");
    });
    // console.log("dragStarted", this, draggingElement);
}

function dragEnd() {
    $(".dropPart").each(function () {
        $(this).removeClass("active");
    });
}

function dragOver(e) {
    // console.log("over", this);
    e.preventDefault();
}

function dragEnter(e) {
    // console.log("enter", this);
    e.preventDefault();
}

function dragLeave() {
    // console.log("leave", this);
}

function dragDrop() {
    if ($(this).is($(draggingElement).next()) ||
        $.contains(draggingElement[0], $(this)[0]))
        return;

    $(draggingElement).next().remove();

    let newDrop = $("<div/>").addClass("dropPart")[0];
    $(this).after(newDrop);
    $(this).after(draggingElement);

    newDrop.addEventListener('dragover', dragOver);
    newDrop.addEventListener('dragenter', dragEnter);
    newDrop.addEventListener('dragleave', dragLeave);
    newDrop.addEventListener('drop', dragDrop);
}