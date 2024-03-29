﻿function collectBodyData() {
    let fieldNameArray = [];
    let fieldTypeArray = [];
    let inputs = document.getElementsByName("fieldName");
    let selects = document.getElementsByName("fieldType");
    let fileFormat = document.getElementById("fileFormat");
    let totalRows = document.getElementById("totalRows");


    for (i = 0; i < inputs.length; i++) {
        fieldNameArray.push(inputs[i].value)
        fieldTypeArray.push(selects[i].value)
    }

    let jsonBodyData = JSON.stringify({
        names: fieldNameArray.join(),
        types: fieldTypeArray.join(),
        format: fileFormat.value,
        rowsCount: totalRows.value,
    });

    let obj = JSON.parse(jsonBodyData);

    return obj;
}

function checkIfDuplicateExists(element) {
    let fieldNameArray = [];

    for (let i = 0; i < element.length; i++) {
        let loverCaseVal = String.prototype.toLowerCase.call(element[i].value);
        fieldNameArray.push(loverCaseVal);
    }

    return new Set(fieldNameArray).size !== fieldNameArray.length
}

function validateForm() {
    let isValid = true;
    let idCounter = 0;
    let inputs = document.getElementsByName("fieldName");
    let selects = document.getElementsByName("fieldType");
    let fileFormat = document.getElementById("fileFormat");
    let totalRows = document.getElementById("totalRows");

    if (inputs.count != selects.count) {
        isValid = false;
        return isValid;
    }

    if (checkIfDuplicateExists(inputs)) {
        isValid = false;
    }

    for (i = 0; i < inputs.length; i++) {

        // 1. Field Name validation
        if (inputs[i].value == "" && inputs[i].classList.contains("needs_validation")) {
            isValid = false;
        }
        else if (inputs[i].value == "" && !inputs[i].classList.contains("needs_validation")) {
            inputs[i].classList.add("needs_validation");
            isValid = false;
        }
        else if (inputs[i].value != "" && inputs[i].classList.contains("needs_validation")) {
            inputs[i].classList.remove("needs_validation");
        }

        // 2. Field Type validation
        if (selects[i].value == "" || selects[i].value == "--Select--" && selects[i].classList.contains("needs_validation")) {
            isValid = false;
        }
        else if (selects[i].value == "" || selects[i].value == "--Select--" && !selects[i].classList.contains("needs_validation")) {
            selects[i].classList.add("needs_validation");
            isValid = false;
        }
        else if (selects[i].value != "" || selects[i].value != "--Select--" && selects[i].classList.contains("needs_validation")) {
            selects[i].classList.remove("needs_validation");
        }

        // 3. Counts of ID types.
        if (selects[i].value == "ID") {
            idCounter++;
        }

        if (idCounter > 1) {
            isValid = false;
        }
    }

    // 4. Output Format validation
    if (fileFormat == "" && !fileFormat.classList.contains("needs_validation")) {
        fileFormat.classList.add("needs_validation");
        isValid = false;
    }
    else if (fileFormat != "" && fileFormat.classList.contains("needs_validation")) {
        fileFormat.classList.remove("needs_validation");
    }
    else if (fileFormat == "" && fileFormat.classList.contains("needs_validation")) {
        isValid = false;
    }

    // 5. Total Rows validation
    if (totalRows.value < 1 || totalRows.value > 1000) {
        totalRows.classList.add("needs_validation");
        isValid = false;
    }
    else if (totalRows.value > 0 && totalRows.value < 1001 && totalRows.classList.contains("needs_validation")) {
        totalRows.classList.remove("needs_validation");
    }
    else if (totalRows.value < 1 || totalRows.value > 1000 && totalRows.classList.contains("needs_validation")) {
        isValid = false;
    }

    return isValid;
}

function requestDataGeneration() {
    let bodyData = collectBodyData();

    if (!validateForm()) {
        alert("We're sorry, but the form cannot be submitted, here is few tips to help you troubleshoot the issue:\n\n" +
            "1. Please fill in all the required, red border fields before proceeding.\n" +
            "2. Please ensure that you select only one \"ID\" data type in \"Field Type\" section.\n" +
            "3. Please make sure that you have only unique names in \"Field Name\" section\n" +
            "4. Please take into consideration that the number of rows should range from 1 to 1000.\n");
        return;
    }

    fetch('/Generator/GetAll?' + new URLSearchParams(bodyData), {
        method: 'GET',
    })
        .then(function (response) {
            if (!response.ok) {
                return response.text();
            }
            else {
                return response.blob();
            }
        })
        .then(function (data) {
            if (data instanceof Blob) {
                /*console.log(data);*/

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
            }
            else {
                alert(data);
            }
        })
        .catch(function (error) {
            console.log(error);
        });
}