async function loadCurrenciesAsync()
{
    var url = 'https://localhost:44359/CurrencyCalculator/ListCurrenciesAsync';
    var response = await fetch(url);
    var data = await response.json();

    return data;
}

async function convertAsync(fromCurrencyId, toCurrencyId, amount, exchangeDate)
{
    var url = 'https://localhost:44359/CurrencyCalculator/ConvertAsync/' + amount + '/' + exchangeDate + '/' + fromCurrencyId + '/' + toCurrencyId;
    var response = await fetch(url);
    var data = await response.json();
    
    return data;
}

async function totalAsync(request)
{
    var url = 'https://localhost:44359/CurrencyCalculator/TotalAsync';
    var response = await fetch(url, {
        method: 'post',
        headers: { 
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      });

    var data = await response.json();
    return data;
}

function addRow()
{
    var fromCurrencyId = getCurrencyId("currencyFrom");
    var toCurrencyId = getCurrencyId("currencyTo");
    var exchangeDate = document.getElementById("exchangeDate").value;
    var amount = document.getElementById("amount").value;

    var table = document.getElementById("currencyRows");
    var row = table.insertRow(-1);
    var rowId = new Date().getTime();
    row.id = rowId;

    var exchangeDateCell = row.insertCell(0);
    var amountCell = row.insertCell(1);
    var exchangeRateCell = row.insertCell(2);
    var valueCell = row.insertCell(3);
    var buttonCell = row.insertCell(4);
    var fromCurrencyCodeCell = row.insertCell(5);
    fromCurrencyCodeCell.style.visibility = 'hidden';

    exchangeDateCell.textContent = exchangeDate;
    amountCell.textContent = amount + ' ' + getCurrencyName("currencyFrom");;
    convertAsync(fromCurrencyId, toCurrencyId, amount, exchangeDate)
    .then(data => 
        {
            exchangeRateCell.textContent = data.exchangeRate;
            valueCell.textContent = data.value + ' ' + getCurrencyName("currencyTo");
        });
    
    buttonCell.appendChild(createButton(rowId));
    fromCurrencyCodeCell.textContent = fromCurrencyId;
}

function getCurrencyId(id)
{
    var currency = document.getElementById(id);
    return currency.options[currency.selectedIndex].value;
}

function getCurrencyName(id)
{
    var currency = document.getElementById(id);
    return currency.options[currency.selectedIndex].text;
}

function createButton(id)
{
    var button = document.createElement("button");  
    button.appendChild(document.createTextNode("Ta bort"));
    button.setAttribute("id", id);
    button.setAttribute("onclick", "deleteRow(id)");
    return button;
}

function deleteRow(id)
{
    var table = document.getElementById("currencyRows");

    for (var i = 0, row; row = table.rows[i]; i++) 
    {
        if(row.id === id)
        {
            table.deleteRow(i);
        }
    }
}

function initializePage()
{
    loadCurrenciesAsync().then(data =>
        {
            populate("currencyFrom", data);
            populate("currencyTo", data);
            populate("currencyTotal", data);
        });
}

function populate(id, data)
{
    var dropdown = document.getElementById(id);

    for (var i = 0; i < data.length; i++) {
        var option = document.createElement("option");
        option.text = data[i].code + ' (' + data[i].name + ')';
        option.value = data[i].id;
        dropdown.add(option);
    }
}

function calculateTotal()
{
    var currency = document.getElementById('currencyTotal');
    var toCurrencyId = currency.options[currency.selectedIndex].value;

    if(toCurrencyId !== '')
    {
        var request = 
        {  
            toCurrencyId: toCurrencyId,
            amounts: []
        };
        
        var table = document.getElementById("currencyRows");

        for (var i = 1, row; row = table.rows[i]; i++) 
        {
            var amount = row.cells[1].textContent;
            var fromCurrencyId = row.cells[5].textContent;
        
            request.amounts.push({ 
                "value" : parseFloat(amount),
                "currencyId"  : fromCurrencyId
            });
        }

        totalAsync(request)
        .then(data => 
        {
            var total = document.getElementById("totalAmount");
            total.textContent = data;
        });
    }
}
