$(document).ready(function ()
{
    // New field
    let select = document.getElementsByName("fieldType");
    setSelectionOptions(select);
});

function toggleMenu() {
    let toggleBtnIcon = document.querySelector('.toggle_btn i')
    let dropdownMenu = document.querySelector('.dropdown_menu')

    dropdownMenu.classList.toggle('open');
    let isOpen = dropdownMenu.classList.contains('open');

    toggleBtnIcon.classList = isOpen ? 'fa-solid fa-xmark' : 'fa-solid fa-bars';    
}

function setSelectionOptions(element) {
    const Categories = {
        1: "ID",
        2: "Name",
        3: "Surname",
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
    let firstCell = newRow.insertCell(0);
    let secondCell = newRow.insertCell(1);
    let thirdCell = newRow.insertCell(2);
    let fourthCell = newRow.insertCell(3);

    select = setSelectionOptions(select);
    //New field
    select.name = "fieldType";
    input.placeholder = "Your field name";
    //New field
    input.name = "fieldName";
    addButton.innerHTML = "Add";
    addButton.classList += "add_btn";
    addButton.onclick = function () {
        addTableField(this);
    };
    removeButton.innerHTML = "Remove";
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

function collectBodyData() {
    let fieldNameArray = [];
    let fieldTypeArray = [];
    let inputs = document.getElementsByName("fieldName");
    let selects = document.getElementsByName("fieldType");
    let fileFormat = document.getElementsByName("fileFormat");
    

    for (i = 0; i < inputs.length; i++) {
        fieldNameArray.push(inputs[i].value)
        fieldTypeArray.push(selects[i].value)
    }

    let jsonBodyData = JSON.stringify({
        names: fieldNameArray.join(),
        types: fieldTypeArray.join(),
        format: fileFormat[0].value
    });

    let obj = JSON.parse(jsonBodyData);

    return obj;
}

function requestDataGeneration() {
    let bodyData = collectBodyData();
    
    console.log(bodyData);

    $.ajax({
        type: "GET",
        url: "/Generator/GetAll",
        data: bodyData,
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log(data);  

            var link = document.createElement('a');
            var url = URL.createObjectURL(data);

            if (data.type === "text/plain") {
                link.setAttribute('download', "synthdata.json");
            }
            else {
                link.setAttribute('download', "synthdata");
            }

            link.setAttribute('href', url);
            link.style.display = 'none';

            document.body.appendChild(link);

            link.click();

            document.body.removeChild(link);
        },
        error: function (error) {
            alert("Something goin wrong:" + error.fail);
            console.log(error);
        }
    });
}



