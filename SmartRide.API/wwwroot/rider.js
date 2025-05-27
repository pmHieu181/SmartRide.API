// Thay đổi cổng 7035 thành cổng chính xác của bạn
const API_BASE_URL = "https://localhost:7035";

// Lấy các element từ HTML
const bookingForm = document.getElementById('bookingForm');
const resultMessageDiv = document.getElementById('result-message');
const modal = document.getElementById('confirmationModal');
const confirmationText = document.getElementById('confirmationText');
const okBtn = document.getElementById('okBtn');

// Gán sự kiện cho form
bookingForm.addEventListener('submit', async (event) => {
    event.preventDefault(); // Ngăn form tự động gửi đi
    await bookRide();
});

// Gán sự kiện cho nút OK trên modal
okBtn.addEventListener('click', () => {
    modal.style.display = 'none'; // Ẩn modal đi
});

async function bookRide() {
    resultMessageDiv.innerText = ''; // Xóa thông báo lỗi cũ

    // Lấy dữ liệu từ các ô input
    const passengerId = document.getElementById('passengerId').value;
    const pickupLat = document.getElementById('pickupLat').value;
    const pickupLon = document.getElementById('pickupLon').value;
    const dropoffLat = document.getElementById('dropoffLat').value;
    const dropoffLon = document.getElementById('dropoffLon').value;

    const requestBody = {
        passengerId: parseInt(passengerId),
        pickup: { latitude: parseFloat(pickupLat), longitude: parseFloat(pickupLon) },
        dropoff: { latitude: parseFloat(dropoffLat), longitude: parseFloat(dropoffLon) }
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/rides/book`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
        });

        if (response.ok) {
            const rideData = await response.json();
            // Hiển thị popup xác nhận
            confirmationText.innerText = `Ride Confirmed with driver ${rideData.driver.name}`;
            modal.style.display = 'flex';
        } else {
            const errorText = await response.text();
            resultMessageDiv.innerText = `Error: ${errorText}`;
        }
    } catch (error) {
        resultMessageDiv.innerText = 'Network error. Could not connect to the API.';
    }
}