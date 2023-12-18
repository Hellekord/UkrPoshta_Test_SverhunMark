document.addEventListener('DOMContentLoaded', function () {
    // Remove fetchDepartments call since we're not using the dropdown
    setupReportGeneration();
    setupReportExport();
});

// Remove fetchDepartments function completely

function setupReportGeneration() {
    const generateButton = document.getElementById('generate-report-button');
    generateButton.addEventListener('click', () => {
        const departmentName = document.getElementById('department-input').value; // Changed from department-select to department-input
        fetchSalaryReport(departmentName);
    });
}

function fetchSalaryReport(departmentName) {
    fetch(`/api/employees/salaryreport?departmentName=${encodeURIComponent(departmentName)}`)
        .then(response => response.text()) // Use .text() instead of .json() if the response is plain text
        .then(report => {
            displayReport(report);
        })
        .catch(error => console.error('Failed to generate salary report', error));
}

function displayReport(reportText) {
    const reportContainer = document.getElementById('report-container');
    reportContainer.innerHTML = '';
    // If the report is plain text with newlines, split it into lines
    const lines = reportText.split('\n');
    lines.forEach(line => {
        let p = document.createElement('p');
        p.textContent = line;
        reportContainer.appendChild(p);
    });
}


function exportSalaryReport(departmentName) {
    // Update the endpoint to include the query parameter for departmentName
    fetch(`/api/employees/exportSalaryReport?departmentName=${encodeURIComponent(departmentName)}`)
        .then(response => response.blob()) // Convert the response to a blob for download
        .then(blob => {
            // Create a URL for the blob
            const url = window.URL.createObjectURL(blob);
            // Create an anchor element and set the download attribute with the file name
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = `${departmentName}_salary_report.txt`;
            // Append the anchor to the body, click it to trigger the download, and then remove it
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(error => console.error('Failed to export salary report', error));
}

// You also need to update the event listener for the export button to get the department name from the input
function setupReportExport() {
    const exportButton = document.getElementById('export-report-button');
    exportButton.addEventListener('click', () => {
        const departmentName = document.getElementById('department-input').value; // Assuming you have an input field with this id
        exportSalaryReport(departmentName);
    });
}

