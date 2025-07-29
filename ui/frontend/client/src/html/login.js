// login.js
import React, { useState } from 'react';
import '../css/login.css';
import Header from '../components/Header';
import { useAuth } from '../context/AuthContext';

function Login() {
  const { login } = useAuth();

  const [isLoginMode, setIsLoginMode] = useState(true);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    username: '',
    email: '',
    password: '',
    phoneNumber: '',
  });

  const [errorMessage, setErrorMessage] = useState('');

  const toggleMode = () => {
    setIsLoginMode(!isLoginMode);
    setFormData({
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      password: '',
      phoneNumber: '',
    });
    setErrorMessage('');
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');

    const endpoint = isLoginMode
      ? 'https://localhost:7203/api/User/Login'
      : 'https://localhost:7203/api/User/RegisterUser';

    const payload = isLoginMode
      ? {
          username: formData.username,
          password: formData.password,
        }
      : {
          first_name: formData.firstName,
          last_name: formData.lastName,
          username: formData.username,
          email: formData.email,
          password: formData.password,
        };

    try {
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      console.log(`[${isLoginMode ? 'Login' : 'Signup'}] Response status:`, response.status);

      if (!response.ok) {
        const text = await response.text().catch(() => '');
        console.error(`[${isLoginMode ? 'Login' : 'Signup'}] Error text:`, text);

        if ([400, 401, 409].includes(response.status)) {
          setErrorMessage('Invalid input or credentials. Please check your information.');
        } else {
          setErrorMessage('Server error. Please try again later.');
        }
        return;
      }

      const data = await response.json();
      console.log(`[${isLoginMode ? 'Login' : 'Signup'}] Success:`, data);

      if (isLoginMode) {
        login({
          user_id: data.user.user_id,
          name: `${data.user.first_name} ${data.user.last_name}`,
          email: data.user.email,
          token: data.token
        });
        alert(`Logged in as ${data.user.username}`);
      } else {
        alert('Account created successfully!');
        setIsLoginMode(true);
      }
    } catch (err) {
      console.error(`[${isLoginMode ? 'Login' : 'Signup'}] Network error:`, err);
      setErrorMessage('Network error. Please try again later.');
    }
  };

  return (
    <>
      <Header />
      <div className="login-container">
        <div className="login-box">
          <div className="login-header">
            <h1>{isLoginMode ? 'Login to Your Account' : 'Create an Account'}</h1>
          </div>

          {errorMessage && <div className="error-message">{errorMessage}</div>}

          <form onSubmit={handleSubmit}>
            {!isLoginMode && (
              <>
                <label htmlFor="firstName">First Name</label>
                <input
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                />
                <label htmlFor="lastName">Last Name</label>
                <input
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                />
              </>
            )}

            <label htmlFor="username">Username</label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
            />

            {!isLoginMode && (
              <>
                <label htmlFor="email">Email</label>
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  required
                />
                <label htmlFor="phoneNumber">Phone Number</label>
                <input
                  type="tel"
                  name="phoneNumber"
                  value={formData.phoneNumber}
                  onChange={handleChange}
                />
              </>
            )}

            <label htmlFor="password">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
            />

            {isLoginMode && (
              <div className="forgot-links">
                <button type="button" onClick={() => alert('Forgot Username clicked')}>
                  Forgot Username?
                </button>
                <button type="button" onClick={() => alert('Forgot Password clicked')}>
                  Forgot Password?
                </button>
              </div>
            )}

            <div className="login-actions">
              <button type="submit" className="login-btn">
                {isLoginMode ? 'Login' : 'Sign Up'}
              </button>
              <button
                type="button"
                className="signup-btn"
                onClick={toggleMode}
              >
                {isLoginMode ? 'Create an account' : 'Back to login'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}

export default Login;

