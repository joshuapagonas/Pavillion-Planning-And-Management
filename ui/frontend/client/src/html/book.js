// Book.js
// Run: npm install react-datepicker date-fns
import React, { useState, useEffect } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import '../css/book.css';
import Header from '../components/Header';
import { useAuth } from '../context/AuthContext';

// If you're using mock data for testing:
import testData from '../JSON_test/booking.json';

const parkList = testData.parks; // Or define inline
const timeSlots = testData.timeSlots;

function Book() {
  const [selectedPark, setSelectedPark] = useState(null);
  const [selectedDate, setSelectedDate] = useState(null);
  const [selectedSlot, setSelectedSlot] = useState(null);
  const [errorMessage, setErrorMessage] = useState('');
  const [bookedDates, setBookedDates] = useState([]);

  const { user, isAuthenticated } = useAuth();

  useEffect(() => {
    // Simulate fetching booked dates
    const convertedDates = testData.bookedDates.map(dateStr => new Date(dateStr));
    setBookedDates(convertedDates);
  }, []);

  const openBooking = (park) => {
    if (!isAuthenticated) {
      alert("You must be logged in to book a pavilion.");
      return;
    }
    setSelectedPark(park);
    setSelectedDate(null);
    setSelectedSlot(null);
    setErrorMessage('');
  };

  const closeBooking = () => {
    setSelectedPark(null);
    setSelectedDate(null);
    setSelectedSlot(null);
    setErrorMessage('');
  };

  const handleSelectSlot = (slot) => {
    setSelectedSlot(slot);
    setErrorMessage('');
  };

  const handleConfirm = () => {
    if (!selectedDate) {
      setErrorMessage('Please select a date.');
      return;
    }
    if (!selectedSlot) {
      setErrorMessage('Please select a time slot.');
      return;
    }
    alert(
      `Booking confirmed for ${selectedPark} on ${selectedDate.toLocaleDateString()} (${selectedSlot.label})`
    );
    closeBooking();
  };

  return (
    <>
      <Header />
      <div className="p-6 max-w-4xl mx-auto">
        <h1 className="text-2xl font-bold mb-6">Book a Pavilion</h1>
        <table className="w-full border-collapse">
          <thead>
            <tr className="bg-blue-100 text-left">
              <th className="p-3 border">Park</th>
              <th className="p-3 border">Action</th>
            </tr>
          </thead>
          <tbody>
            {parkList.map((park, i) => (
              <tr key={i} className="bg-white hover:bg-gray-50">
                <td className="p-3 border font-medium">{park}</td>
                <td className="p-3 border">
                  <button
                    className="text-blue-600 hover:underline"
                    onClick={() => openBooking(park)}
                  >
                    Book now!
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedPark && (
        <div className="booking-popup">
          <h2 className="text-lg font-bold mb-3">Book {selectedPark}</h2>

          <p className="mb-1"><strong>Name:</strong> {user?.name}</p>
          <p className="mb-4"><strong>Email:</strong> {user?.email}</p>

          <label className="block mb-4 font-medium">Select a date:</label>
          <DatePicker
            inline
            selected={selectedDate}
            onChange={(date) => setSelectedDate(date)}
            excludeDates={bookedDates}
            minDate={new Date()}
          />

          <label className="block mb-3 font-medium">Choose a time slot:</label>
          <div className="timeslot-grid mb-4">
            {timeSlots.map((slot) => (
              <button
                key={slot.label}
                className={`timeslot-button ${selectedSlot === slot ? 'selected' : ''}`}
                onClick={() => handleSelectSlot(slot)}
              >
                {slot.label}
              </button>
            ))}
          </div>

          {errorMessage && (
            <div className="text-red-600 mb-3">{errorMessage}</div>
          )}

          <div className="button-row">
            <button onClick={closeBooking} className="btn-cancel">
              Cancel
            </button>
            <button
              onClick={handleConfirm}
              className="btn-confirm"
            >
              Confirm Booking
            </button>
          </div>
        </div>
      )}
    </>
  );
}

export default Book;
