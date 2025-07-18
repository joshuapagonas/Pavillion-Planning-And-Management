// ParkEvents.js
import React from 'react';
import '../css/ParkEvents.css';
import Header from '../components/Header';
import { useAuth } from '../context/AuthContext';

const events = [
  {
    title: 'Disc Golf Tournament',
    date: 'July 15, 2025',
    spots: 25,
  },
  {
    title: 'Community Easter Egg Hunt',
    date: 'April 12, 2025',
    spots: 100,
  },
  {
    title: 'Northern Lights Show',
    date: 'December 2, 2025',
    spots: 200,
  },
];

function ParkEvents() {
  const { user, isAuthenticated } = useAuth();
  function openBooking () {
    if (!isAuthenticated) {
      alert("You must be logged in to register for an event.");
      return;
    }
  }
  
  return (
    <>
      <Header />
      <div className="events-container">
        <h1 className="events-title">Upcoming Registerable Park Events</h1>
        <div className="event-list">
          {events.map((event, idx) => (
            <div key={idx} className="event-card">
              <h2>{event.title}</h2>
              <p><strong>Date:</strong> {event.date}</p>
              <p><strong>Spots Available:</strong> {event.spots}</p>
              <button className="register-btn"
              onClick={() => openBooking()}
              >Register</button>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}
  
export default ParkEvents;