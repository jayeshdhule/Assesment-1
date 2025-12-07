import { render, screen } from '@testing-library/react';
import App from './App';

test('renders company title', () => {
  render(<App />);
  const titleElement = screen.getByText(/Avalpha Technologies/i);
  expect(titleElement).toBeInTheDocument();
});
