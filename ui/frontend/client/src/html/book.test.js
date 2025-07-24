// Book.test.js
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Book from './book';
import { AuthContext } from '../context/AuthContext';
import testData from '../test/book.json';
import { MemoryRouter } from 'react-router-dom';


// Mock AuthContext with a user
const mockUser = {
  name: testData.user.name,
  email: testData.user.email
};

const renderWithAuth = (ui) => {
  return render(
    <AuthContext.Provider value={{ user: mockUser, isAuthenticated: true }}>
      {ui}
    </AuthContext.Provider>
  );
};

describe('Book Component', () => {
  test('renders park list', () => {
    renderWithAuth(<Book />);
    testData.parks.forEach((park) => {
      expect(screen.getByText(park)).toBeInTheDocument();
    });
  });

  test('opens booking popup when clicking "Book now!"', () => {
    renderWithAuth(<Book />);
    const bookButton = screen.getAllByText('Book now!')[0];
    fireEvent.click(bookButton);
    expect(screen.getByText(/Book /)).toBeInTheDocument();
    expect(screen.getByText(mockUser.name)).toBeInTheDocument();
  });

  test('shows error if date or time slot not selected', () => {
    renderWithAuth(<Book />);
    fireEvent.click(screen.getAllByText('Book now!')[0]);

    const confirmButton = screen.getByText('Confirm Booking');
    fireEvent.click(confirmButton);

    expect(screen.getByText('Please select a date.')).toBeInTheDocument();
  });
});
