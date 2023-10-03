
$(document).ready(function () {
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
                // Generate HTML table to display search results
                // ...
            // },  Conso
            // data: { query: query, property: property, entity: entity },
            // success: function (data) {
                // Clear the search results container
                $('#searchResultsContainer').empty();

                // Check if there are results
                if (data.length === 0) {
                    $('#searchResultsContainer').html('<p>No results found.</p>');
                } else  if (entity=='company'){
                    console.log('XXXXXXXXXXXXXXXXX Inside comp if: data passed is', data);

                    var table = $('<table class="table table-hover"></table>');
                   
                    // Populate the table with search results
                    // if (entity === 'Company'){
                    
                        var headerRow = $('<tr><th>Name</th><th>CRNo</th><th>TaxID</th><th>Industry</th><th>Type</th></tr>');
                        table.append(headerRow);
    
                    $.each(data, function (index, company) {
                        var row = $('<tr></tr>');
                        row.append('<td><a href="/Company/CompanyDetails/?crNo=' + company.crNo + '">' + company.companyName + '</a></td>');
                        row.append('<td>' + company.crNo + '</td>');
                        row.append('<td>' + company.taxID + '</td>');
                        row.append('<td>' + company.industry + '</td>');
                        row.append('<td>' + company.type+ '</td>');

                        // Add more columns as needed

                        table.append(row);
                        // $('#searchResultsContainer').append(table);

                    });
                    // $('#searchResultsContainer').append(table);
                } else if (entity=='financerequest'){
                    console.log('oooooooooooooooo inside fr if and data is :', data);
                    var table = $('<table class="table table-hover"></table>');
                   
                    // Populate the table with search results
                    // if (entity === 'Company'){
                    
                        var headerRow = $('<tr><th>RequestNo</th><th>Company</th><th>Amount</th><th>Tenor</th><th>Facility</th><th>Status</th></tr>');
                        table.append(headerRow);
    
                    $.each(data, function (index, financeRequest) {
                        var row = $('<tr></tr>');
                        row.append('<td><a href="/Company/CompanyDetails/?crNo=' + financeRequest.crNo + '">' + financeRequest.company.companyName + '</a></td>');
                        row.append('<td>' + financeRequest.requestNo + '</td>');
                        row.append('<td>' + financeRequest.company.companyName + '</td>');
                        row.append('<td>' + financeRequest.amount + '</td>');
                        row.append('<td>' + financeRequest.tenor + '</td>');
                        row.append('<td>' + financeRequest.faciltyType + '</td>');
                        row.append('<td>' + financeRequest.status + '</td>');
                        // Add more columns as needed

                        table.append(row);
                        // $('#searchResultsContainer').append(table);

                    });
                    // $('#searchResultsContainer').append(table);
                }
                $('#searchResultsContainer').append(table);
                
            },
            error: function () {
                $('#searchResultsContainer').html('<p>Error occurred while searching.</p>');
            }
        });
    }
                // }
            //     else if (entity === 'FinanceRequest'){
            //         var headerRow = $('<tr><th>Name</th><th>CRNo</th><th>TaxID</th><th>Industry</th><th>Type</th></tr>');
            //         table.append(headerRow);
            //         $.each(data, function (index, company) {
            //         row.append('<td><a href="/FinanceRequest/Details/?requestNo' + financeRequest.requestNo + '">' + financeRequest.companyName + '</a></td>');
            //         row.append('<td>' + financeRequest.requestNo + '</td>');
            //         row.append('<td>' + financeRequest.companyName + '</td>');
            //         row.append('<td>' + financeRequest.facilityType + '</td>');
            //         row.append('<td>' + financeRequest.amount+ '</td>');
            //         row.append('<td>' + financeRequest.tenor+ '</td>');
            //         row.append('<td>' + financeRequest.status+ '</td>');
            //         table.append(row);

            //     });
            // }

                    // Append the table to the search results container


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
//                     // Create a table or format the results as needed
//                     // For example, you can use a loop to display data
//                     var resultsHtml = '<table class="table table-hover"><thead><tr><th>Entity</th><th>Property</th><th>Value</th></tr></thead><tbody>';
//                     $.each(data, function (_, result) {
//                         // Check if the property value contains the search query
//                         if (result[property].toLowerCase().includes(query.toLowerCase())) {
//                             resultsHtml += '<tr><td>' +''+'</td><td>' + property + '</td><td>' + result + '</td></tr>';
//                         }
//                     });
//                     resultsHtml += '</tbody></table>';

//                     // Append the table to the search results container
//                     $('#searchResultsContainer').html(resultsHtml);
//                 }
//             },
//             error: function () {
//                 $('#searchResultsContainer').html('<p>Error occurred while searching.</p>');
//             }
//         });
    
//     }
//     // Handle the search button click event
//     $('#searchButton').click(function () {
//         performSearch();
//     });

//     // Handle the enter button event
//     $('#searchQuery').on('keydown', function (event) {
//         if (event.keyCode === 13) { // Check for Enter key
//             performSearch();
//         }
//     });
// });
