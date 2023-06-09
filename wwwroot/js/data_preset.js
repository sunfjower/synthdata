function collectBodyData() {
    let fieldNameArray = [];
    let fieldTypeArray = [];
    let inputs = document.getElementsByName("fieldName");
    let selects = document.getElementsByName("fieldType");
    let fileFormat = document.getElementById("fileFormat");


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