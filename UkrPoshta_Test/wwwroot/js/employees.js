document.addEventListener('DOMContentLoaded', function () {
    setupExportButton();
    setupSearchFilters();
    setupEditEmployeeListener(); // This will now properly set up the listener
});

function setupExportButton() {
    const exportButton = document.getElementById('export-data-button');
    exportButton.addEventListener('click', exportEmployeeData);
}

function exportEmployeeData() {
    fetch('/api/employees/export', {
        method: 'GET', // or 'POST' if you need to send data for the export
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob(); // or .text() if it's returning plain text
        })
        .then(blob => {
            // Create a Blob from the PDF Stream
            const url = window.URL.createObjectURL(blob);
            // Create a link to download it
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            // the filename you want
            a.download = 'employees_export.csv';
            document.body.appendChild(a);
            a.click();
            // Clean up and remove the link
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

function setupSearchFilters() {
    const searchButton = document.getElementById('search-button');
    searchButton.addEventListener('click', searchEmployees);
}

function searchEmployees() {
    // Get the values from the search input fields
    const nameInput = document.getElementById('search-name').value;
    const departmentInput = document.getElementById('search-department').value;
    const positionInput = document.getElementById('search-position').value;

    // Construct the search query parameters
    const queryParams = new URLSearchParams({
        fullName: nameInput,
        departmentName: departmentInput,
        positionName: positionInput
    });

    // Send the search request to the server
    fetch(`/api/employees/search?${queryParams.toString()}`, {
        method: 'GET'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            // Process the response data
            updateEmployeeTable(data); // Assume this function updates the UI with the results
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
            // Optionally, update the UI to inform the user that an error occurred
        });
}

// Helper function to update the UI with search results
function updateEmployeeTable(employees) {
    const tbody = document.getElementById('employee-list').getElementsByTagName('tbody')[0];
    tbody.innerHTML = ''; // Clear any existing rows

    employees.forEach(employee => {
        let row = tbody.insertRow(); // Insert a new row in the table

        // Insert cells and fill them with employee data
        row.insertCell(0).innerText = employee.fullName || 'N/A';
        row.insertCell(1).innerText = employee.departmentName || 'N/A';
        row.insertCell(2).innerText = employee.positionName || 'N/A';
        row.insertCell(3).innerText = employee.salary ? `$${employee.salary.toFixed(2)}` : 'N/A';

        let editCell = row.insertCell(4);
        let editButton = document.createElement('button');
        editButton.classList.add('edit-employee-btn'); // Add class for styling and selection
        editButton.textContent = 'Edit';
        editButton.setAttribute('data-employee-id', employee.employeeID);
        editButton.onclick = function() { handleEditEmployee(employee.employeeID); };
        editCell.appendChild(editButton);

        // If you have an 'Edit' button or other interactive elements, add them here
    });
    document.getElementById('search-button').addEventListener('click', searchEmployees);

}


function setupEditEmployeeListener() {
    // This will delegate the click event from the document to the edit buttons
    document.addEventListener('click', function (event) {
        if (event.target && event.target.matches('.edit-employee-btn')) {
            const employeeId = event.target.getAttribute('data-employee-id');
            handleEditEmployee(employeeId);
        }
    });
}

function handleEditEmployee(employeeId) {
    fetch(`/api/employees/${employeeId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(employee => {
            populateEditForm(employee);
        })
        .catch(error => {
            console.error('Error fetching employee data:', error);
            alert('Error fetching employee data. Please try again.');
        });
}

function populateEditForm(employee) {
    // Assume you have form fields with IDs corresponding to the employee properties
    document.getElementById('edit-id').value = employee.employeeId;
    document.getElementById('edit-fullname').value = employee.fullName;
    document.getElementById('edit-department').value = employee.departmentName || '';
    document.getElementById('edit-position').value = employee.positionName || '';
    document.getElementById('edit-salary').value = employee.salary || '';
    document.getElementById('edit-form-container').style.display = 'block';
}

function showEditForm() {
    // Display the edit form, you might have a specific logic to show the form
    const editFormContainer = document.getElementById('edit-form-container');
    editFormContainer.style.display = 'block';
}


function saveEmployeeData(employeeId, updatedData) {
    // Assuming you have functions to resolve department and position names to IDs
    Promise.all([
        getDepartmentIdByName(updatedData.departmentName),
        getPositionIdByName(updatedData.positionName)
    ])
        .then(([departmentId, positionId]) => {
            if (departmentId && positionId) {
                const dataWithIds = {
                    ...updatedData,
                    departmentId,
                    positionId
                };

                return fetch(`/api/employees/${employeeId}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(dataWithIds)
                });
            } else {
                throw new Error('Failed to get department or position ID');
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(() => {
            alert('Employee updated successfully.');
            // Refresh the employee list or handle as needed
        })
        .catch(error => {
            console.error(`Failed to update employee: ${error}`);
            alert(`Failed to update employee: ${error}`);
        });

    document.getElementById('save-employee-btn').addEventListener('click', function () {
        const employeeId = document.getElementById('edit-id').value;
        saveEmployeeData(employeeId);
    });
}
