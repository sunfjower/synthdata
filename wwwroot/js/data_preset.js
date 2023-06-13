function collectBodyData() {
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
        format: fileFormat[0].value,
        rowsCount: totalRows.value,
    });

    let obj = JSON.parse(jsonBodyData);

    return obj;
}

function validateForm() {
    let isValid = true;
    let inputs = document.getElementsByName("fieldName");
    let selects = document.getElementsByName("fieldType");
    let fileFormat = document.getElementById("fileFormat");
    let totalRows = document.getElementById("totalRows");

    if (inputs.count != selects.count) {
        isValid = false;
        return isValid;
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
    }

    // 3. Output Format validation
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

    // 4. Total Rows validation
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
        alert("We're sorry, but the form cannot be submitted with empty fields. " +
            "Please fill in all the required, red border fields before proceeding.\n\n" +
            "Please take into consideration that the number of rows should range from 1 to 1000.");
        return;
    }

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
        xhrFields: {
            responseType: 'text/plain'
        },
        error: function (xhr, status, error) {

            //read like JSON

            console.log(xhr.responseText);
            var err = JSON.parse(xhr.responseText);
            alert(err.message);

/*            msg = Blob.text(xhr.responseText);

            console.log(msg);

            
*/
        }
    });
}