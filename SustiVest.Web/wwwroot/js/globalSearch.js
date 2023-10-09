
$(document).ready(function () {
   // Define properties for each entity
   var properties = {
    'company': [
        { value: 'crNo', text: 'CRNo' },
        { value: 'taxid', text: 'Tax ID' },
        { value: 'companyname', text: 'Company Name' },
        { value: 'industry', text: 'Industry' },
        { value: 'type', text: 'Type' }
    ],
    'financerequest': [
        { value: 'requestno', text: 'Request No' },
        { value: 'financerequest.company.companyname', text: 'Name' },
        { value: 'amount', text: 'Amount' },
        { value: 'tenor', text: 'Tenor' },
        { value: 'facilitytype', text: 'Facility' },
        { value: 'status', text: 'Status' }
    ]
};

// Function to update properties based on the selected entity
function updateProperties() {
    var selectedEntity = $('#searchEntity').val();
    var propertySelect = $('#searchProperty');

    // Clear existing options
    propertySelect.empty();

    // Add options for the selected entity
    properties[selectedEntity].forEach(function (property) {
        propertySelect.append($('<option>', {
            value: property.value,
            text: property.text
        }));
    });
}

// Handle the change event of the searchEntity select element
$('#searchEntity').change(function () {
    updateProperties();
});

// Initial call to set properties based on the initial entity value
updateProperties();

    // Function to perform the search and update the search results
    function performSearch(query, property, entity) {
        // var entity = $('#searchEntity').val();
        // var query = $('#searchQuery').val();
        // var property = $('#searchProperty').val();
        // var entity = $('#searchEntity').val();
    
        $.ajax({
            url: '/GlobalSearch/Search',
            type: 'GET',
            dataType: 'json', // Specify JSON dataType
            data: { query: query, property: property, entity: entity },
            success: function (data) {
                console.log('ZZZZZZZZ top of success function and data is :', data);

                // Clear the search results container
                $('#searchResultsContainer').empty();

                // Check if there are results
                if (data.length === 0) {
                    $('#searchResultsContainer').html('<p>No results found.</p>');
                } else  if (entity=='company'){
                    console.log('XXXXXXXXXXXXXXXXX Inside comp if: data passed is', data);

                    var table = $('<table class="table table-hover"></table>');
                   
                    // Populate the table with search results
                    
                        var headerRow = $('<tr><th>Name</th><th>CRNo</th><th>TaxID</th><th>Industry</th><th>Type</th></tr>');
                        table.append(headerRow);
    
                    $.each(data, function (index, company) {
                        var row = $('<tr></tr>');
                        row.append('<td><a href="/Company/CompanyDetails/?crNo=' + company.crNo + '">' + company.companyName + '</a></td>');
                        row.append('<td>' + company.crNo + '</td>');
                        row.append('<td>' + company.taxID + '</td>');
                        row.append('<td>' + company.industry + '</td>');
                        row.append('<td>' + company.type+ '</td>');


                        table.append(row);

                    });
                } else if (entity=='financerequest'){
                    console.log('oooooooooooooooo inside fr if and data is :', data);
                    var table = $('<table class="table table-hover"></table>');
                   

                        var headerRow = $('<tr><th>RequestNo</th><th>Company</th><th>Amount</th><th>Tenor</th><th>Facility</th><th>Status</th></tr>');
                        table.append(headerRow);
                            // row.append('<td><a asp-action="Details" asp-route-requestNo=' + financeRequest.requestNo + '">' + financeRequest.requestNo+ '</a></td>');

                    $.each(data, function (index, financeRequest) {
                        var row = $('<tr></tr>');
                        row.append('<td><a href="/FinanceRequest/Details?requestNo=' + financeRequest.requestNo + '">'+ financeRequest.requestNo+ '</a></td>');
                        row.append('<td>' + financeRequest.company.companyName + '</td>');
                        row.append('<td>' + financeRequest.amount + '</td>');
                        row.append('<td>' + financeRequest.tenor + '</td>');
                        row.append('<td>' + financeRequest.faciltyType + '</td>');
                        row.append('<td>' + financeRequest.status + '</td>');
                        // Add more columns as needed

                        table.append(row);

                    });
                }
                $('#searchResultsContainer').append(table);
                
            },
            error: function () {
                $('#searchResultsContainer').html('<p>Error occurred while searching.</p>');
            }
        });
    }
    // Handle the search button click event

    $('#searchButton').click(function (event) {
        event.preventDefault(); // Prevent form submission
        var entity = $('#searchEntity').val();
        var query = $('#searchQuery').val();
        var property = $('#searchProperty').val();
        console.log('XXXXXXXXXXXXXXXXX Inside search button click and data passed is', entity, query, property);

        if (query.length === 0) {
            $('#searchResultsContainer').html('<p>Please enter a search word.</p>');
            return;
        }
        performSearch(query, property, entity);
    });

    // Handle the enter button event
    $('#searchQuery').on('keydown', function (event) {
        if (event.keyCode === 13) { // Check for Enter key
            event.preventDefault(); // Prevent form submission
          
            var entity = $('#searchEntity').val();
            var query = $('#searchQuery').val();
            var property = $('#searchProperty').val();
            if (query.length === 0) {
                $('#searchResultsContainer').html('<p>Please enter a search word.</p>');
                return;
            }
            performSearch(query, property, entity);
        }
    });

    
});
