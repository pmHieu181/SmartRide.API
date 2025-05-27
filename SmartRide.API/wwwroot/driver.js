const API_BASE_URL = "https://localhost:7035"; 
const refreshBtn = document.getElementById('refreshBtn');
const ridesTableBody = document.getElementById('ridesTableBody');
const driverIdInput = document.getElementById('driverId');

// Load rides when the page loads or when refresh is clicked
refreshBtn.addEventListener('click', fetchAvailableRides);
document.addEventListener('DOMContentLoaded', fetchAvailableRides);

async function fetchAvailableRides() {
    ridesTableBody.innerHTML = '<tr><td colspan="4">Loading...</td></tr>';
    try {
        const response = await fetch(`${API_BASE_URL}/api/rides/available`);
        if (!response.ok) {
            throw new Error('Failed to fetch available rides.');
        }
        const rides = await response.json();
        renderRides(rides);
    } catch (error) {
        ridesTableBody.innerHTML = `<tr><td colspan="4" style="color: red;">${error.message}</td></tr>`;
    }
}

function renderRides(rides) {
    ridesTableBody.innerHTML = ''; // Clear table
    if (rides.length === 0) {
        ridesTableBody.innerHTML = '<tr><td colspan="4">No available rides at the moment.</td></tr>';
        return;
    }

    rides.forEach(ride => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${ride.passenger.name}</td>
            <td>Lat: ${ride.pickupLocation.latitude.toFixed(3)}, Lon: ${ride.pickupLocation.longitude.toFixed(3)}</td>
            <td>Lat: ${ride.dropoffLocation.latitude.toFixed(3)}, Lon: ${ride.dropoffLocation.longitude.toFixed(3)}</td>
            <td><button class="accept-btn" data-ride-id="${ride.id}">Accept</button></td>
        `;
        ridesTableBody.appendChild(row);
    });
}

// Event delegation to handle clicks on future "Accept" buttons
ridesTableBody.addEventListener('click', async (event) => {
    if (event.target && event.target.classList.contains('accept-btn')) {
        const rideId = event.target.getAttribute('data-ride-id');
        const driverId = driverIdInput.value;

        if (!driverId) {
            alert('Please enter your Driver ID.');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/api/rides/${rideId}/accept`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(parseInt(driverId))
            });

            if (response.ok) {
                alert(`You have accepted ride #${rideId}.`);
                fetchAvailableRides(); // Refresh the list
            } else {
                const errorText = await response.text();
                alert(`Error: ${errorText}`);
            }
        } catch (error) {
            alert('Network error. Could not accept ride.');
        }
    }
});