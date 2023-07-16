$(document).ready(function () {
    // Set options for <select> elements.
    let select = document.getElementsByName("fieldType");
    setSelectionOptions(select);

    //Set event listener for avoiding unwanted characters.
    let inputs = document.getElementsByName("fieldName");
    for (let i = 0; i < inputs.length; i++) {
        inputs[i].addEventListener("input", function (e) {
            avoidUnwantedCharacters(inputs[i]);
        });
    }
});

function avoidUnwantedCharacters(element) {
    let allowedChars = /[^a-z0-9 _]/gi;
    let inputValue = $(element).val();

    if (allowedChars.test(inputValue)) {
        $(element).val(inputValue.replace(allowedChars, ''));
    }
    //$(element).setSelectionRange(c, c);
}

function setSelectionOptions(element) {
    const Categories = {
        1: "ID",
        2: "Name",
        3: "Surname",
        4: "Full Name",
        5: "Email",
        6: "Username",
        7: "Phone",
        8: "Company Name",
        9: "Password",
    };

    for (let i = 0; i <= Object.keys(Categories).length; i++) {
        let option = document.createElement("option");

        if (i === 0) {
            option.text = "--Select--";
            option.defaultSelected = true;
            option.hidden = true;
        }
        else {
            option.text = Categories[i];
        }

        $(element).append(option, i);
    }

    return element;
}

function getCurrentRowIndex(element) {
    let tr = element.parentElement.parentNode;
    let index = tr.rowIndex;

    return index++;
}

function isMaximumFields(element) {
    const maximumFieldsSize = 10;
    let currentFieldsSize = element.rows.length;

    if (currentFieldsSize < maximumFieldsSize) {
        return false;
    } else {
        return true;
    }
}

function addTableField(element) {
    let tbody = document.getElementById("configurationTable").getElementsByTagName("tbody")[0];

    if (isMaximumFields(tbody)) {
        alert("Maximum 10 fields can be added");
        return;
    }

    let newRow = tbody.insertRow(getCurrentRowIndex(element));
    let input = document.createElement("input");
    let select = document.createElement("select");
    let addButton = document.createElement("button");
    let removeButton = document.createElement("button");
    let addSpan = document.createElement("span");
    let removeSpan = document.createElement("span");
    let firstCell = newRow.insertCell(0);
    let secondCell = newRow.insertCell(1);
    let thirdCell = newRow.insertCell(2);
    let fourthCell = newRow.insertCell(3);

    select = setSelectionOptions(select);
    select.name = "fieldType";
    input.placeholder = "Your field name";
    input.name = "fieldName";
    input.maxLength = "50";
    input.addEventListener("input", function (e) {
        avoidUnwantedCharacters(this);
    });
    addSpan.innerHTML = "Add";
    addButton.append(addSpan);
    addButton.classList += "add_btn";
    addButton.onclick = function () {
        addTableField(this);
    };
    removeSpan.innerHTML = "Remove";
    removeButton.append(removeSpan);
    removeButton.classList += "remove_btn";
    removeButton.onclick = function () {
        removeTableField(this);
    };

    firstCell.append(input);
    secondCell.append(select);
    thirdCell.append(addButton);
    fourthCell.append(removeButton);
}

function removeTableField(element) {
    let tr = element.parentElement.parentNode;

    tr.remove();
}