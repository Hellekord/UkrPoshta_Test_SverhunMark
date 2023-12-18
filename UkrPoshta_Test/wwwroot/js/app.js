{
    document.addEventListener('DOMContentLoaded', function () {
        fetchCompanyInfo();
        //fetchEmployees();
    });

    function fetchCompanyInfo() {
        console.log("fetchCompanyInfo called");
        fetch('/api/employees/companyinfo')
            .then(response => response.json())
            .then(data => {
                document.getElementById('company-info').innerText = data.companyDetails; // Make sure this matches the JSON property name from the server
            })
            .catch(error => console.error('Failed to load company information', error));
    }


    

    

    
}