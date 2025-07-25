import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import '../css/mybookings.css';

const MyBookings = () => {
  const { user } = useAuth();
  const [bookings, setBookings] = useState([]);

  useEffect(() => {
    if (user?.user_id) {
      fetch(`http://localhost:5132/api/Registration/user/${user.user_id}`)
        .then((res) => res.json())
        .then((data) => setBookings(data))
        .catch((err) => console.error('Error fetching bookings:', err));
    }
  }, [user]);

  if (!user) {
    return (
      <>
        <Header />
        <div className="my-bookings-container">
          <p>Please log in to view your bookings.</p>
        </div>
      </>
    );
  }

  return (
    <>
      <Header />
      <div className="my-bookings-container">
        <h1 className="my-bookings-title">My Bookings</h1>
        {bookings.length === 0 ? (
          <p className="no-bookings-message">You have no bookings yet.</p>
        ) : (
          <table className="bookings-table">
            <thead>
              <tr>
                <th>Park</th>
                <th>Date</th>
                <th>Time Slot</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {bookings.map((b) => (
                <tr key={b.registration_id}>
                  <td>{b.requested_park}</td>
                  <td>{new Date(b.registration_date).toLocaleDateString()}</td>
                  <td>
                    {new Date(b.start_time).toLocaleTimeString()} -{' '}
                    {new Date(b.end_time).toLocaleTimeString()}
                  </td>
                  <td className={b.is_approved ? 'status-approved' : 'status-pending'}>
                    {b.is_approved ? 'Approved' : 'Pending'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
};

export default MyBookings;

