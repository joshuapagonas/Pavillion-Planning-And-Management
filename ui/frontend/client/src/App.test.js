import { render, screen } from '@testing-library/react';
import App from './App';

test('renders welcome message', () => {
  render(<App />);
  const welcome = screen.getByText(/welcome to pavilion planning/i);
  expect(welcome).toBeInTheDocument();
});

