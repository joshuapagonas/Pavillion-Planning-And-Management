// ParkEvents.js
import React, { useState } from 'react';
import '../css/ParkEvents.css';
import Header from '../components/Header';
import { useAuth } from '../context/AuthContext';
import testData from '../JSON_test/events.json'; 

const ParkEvents = () => {
  const { user, isAuthenticated } = useAuth();

  const [selectedEvent, setSelectedEvent] = useState(null);
  const [reservationName, setReservationName] = useState('');
  const [partySize, setPartySize] = useState('');
  const [userReservations, setUserReservations] = useState(testData.userReservations);
  const events = testData.events;

  const handleRegisterClick = (event) => {
    setSelectedEvent(event);

    // Prefill form if already registered
    const existing = userReservations.find(r => r.eventId === event.id);
    if (existing) {
      setReservationName(existing.name);
      setPartySize(existing.size);
    } else {
      setReservationName('');
      setPartySize('');
    }
  };

  const closeModal = () => {
    setSelectedEvent(null);
    setReservationName('');
    setPartySize('');
  };

  const handleReservationSubmit = (e) => {
    e.preventDefault();

    const updated = userReservations.filter(r => r.eventId !== selectedEvent.id);
    updated.push({
      eventId: selectedEvent.id,
      name: reservationName,
      size: partySize
    });
    setUserReservations(updated);
    closeModal();
  };

  const handleCancelReservation = () => {
    setUserReservations(userReservations.filter(r => r.eventId !== selectedEvent.id));
    closeModal();
  };

  const isRegistered = selectedEvent
    ? userReservations.some(r => r.eventId === selectedEvent.id)
    : false;

  return (
    <>
      <Header />
      <div className="events-container">
        <h1 className="events-title">Upcoming Registerable Park Events</h1>
        <div className="event-list">
          {events.map((event) => (
            <div key={event.id} className="event-card">
              <h2>{event.title}</h2>
              <p><strong>Date:</strong> {event.date}</p>
              <p><strong>Spots Available:</strong> {event.spots}</p>
              <button className="register-btn" onClick={() => handleRegisterClick(event)}>
                Register
              </button>
            </div>
          ))}
        </div>
      </div>

      {selectedEvent && (
        <div className="modal-overlay">
          <div className="modal-content">
            <button className="modal-close" onClick={closeModal}>×</button>

            <h2>{selectedEvent.title}</h2>
            <p><strong>Date:</strong> {selectedEvent.date}</p>

            {!isAuthenticated ? (
              <div>
                <p>You must be logged in to register for this event.</p>
                <a href="/login">Login</a> or <a href="/signup">Sign up</a> to continue.
              </div>
            ) : isRegistered ? (
              <div>
                <p>You have already registered for this event.</p>
                <form onSubmit={handleReservationSubmit}>
                  <label>
                    Update Reservation Name:
                    <input
                      type="text"
                      value={reservationName}
                      onChange={(e) => setReservationName(e.target.value)}
                      required
                    />
                  </label>
                  <label>
                    Update Party Size:
                    <input
                      type="number"
                      value={partySize}
                      onChange={(e) => setPartySize(e.target.value)}
                      min="1"
                      required
                    />
                  </label>
                  <button type="submit">Update Reservation</button>
                  <button
                    type="button"
                    onClick={handleCancelReservation}
                    style={{ backgroundColor: '#bb0000', marginTop: '0.5rem', color: 'white' }}
                  >
                    Cancel Reservation
                  </button>
                </form>
              </div>
            ) : (
              <form onSubmit={handleReservationSubmit}>
                <label>
                  Reservation Name:
                  <input
                    type="text"
                    value={reservationName}
                    onChange={(e) => setReservationName(e.target.value)}
                    required
                  />
                </label>
                <label>
                  Party Size:
                  <input
                    type="number"
                    value={partySize}
                    onChange={(e) => setPartySize(e.target.value)}
                    min="1"
                    required
                  />
                </label>
                <button type="submit">Submit Reservation</button>
              </form>
            )}
          </div>
        </div>
      )}
    </>
  );
};

export default ParkEvents;
