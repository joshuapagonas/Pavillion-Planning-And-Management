// src/components/Header.js
// src/components/Header.js
import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../css/Home.css'; // Adjust this if your nav styles are elsewhere

const Header = () => {
  const { user, isAuthenticated } = useAuth();

  return (
    <header className="header">
      <Link to="/home">
        <img
          src="images/Troy_Homepage.png"
          alt="Troy Michigan Logo"
          className="logo"
        />
      </Link>

      <nav>
        <Link to="/book">Book Now!</Link>
        <Link to="/events">Community Park Events</Link>
        <Link to="/explore">Explore the Parks</Link>
        <Link to="/faq">FAQ</Link>
        <Link to="/contact">Contact us</Link>

        {isAuthenticated && (
          <Link to="/my-bookings">My Bookings</Link>
        )}

        <Link to="/login">Sign up/Login</Link>
      </nav>

      <div className="socials">
        <a href="https://www.facebook.com/TroyMI/"><img src="images/facebook-icon.png" alt="Facebook" /></a>
        <a href="https://x.com/CityTroyMI"><img src="images/twitter-icon.png" alt="Twitter" /></a>
        <a href="https://www.instagram.com/troymichigan/"><img src="images/instagram-icon.png" alt="Instagram" /></a>
        <a href="https://www.youtube.com/TroyMichiganGov"><img src="images/youtube-icon.png" alt="YouTube" /></a>
      </div>
    </header>
  );
};

export default Header;