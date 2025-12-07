import logo from './logo.png';
import './App.css';
import { useState } from 'react';

function App() {
  const [formData, setFormData] = useState({
    localSalesCount: '',
    foreignSalesCount: '',
    averageSaleAmount: ''
  });

  const [results, setResults] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // Prefer an explicit env override. If not set, pick the local API URL based on page protocol
  // - when the UI is served over http (dev), the API in this repo listens on http://localhost:5111
  // - when served over https, the API typically listens on https://localhost:5000
  const API_BASE = process.env.REACT_APP_API_URL || (window.location.protocol === 'https:' ? 'https://localhost:5000' : 'http://localhost:5111'); // adjust if needed
  const currencyFormatter = new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP' });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const validate = () => {
    const local = Number(formData.localSalesCount);
    const foreign = Number(formData.foreignSalesCount);
    const avg = Number(formData.averageSaleAmount);
    if (Number.isNaN(local) || Number.isNaN(foreign) || Number.isNaN(avg)) {
      return 'All inputs must be numbers.';
    }
    if (local < 0 || foreign < 0 || avg < 0) {
      return 'Values must be ≥ 0.';
    }
    if (local > 1_000_000 || foreign > 1_000_000 || avg > 1_000_000_000) {
      return 'One or more values exceed allowed limits.';
    }
    return null;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    const clientValidationError = validate();
    if (clientValidationError) {
      setError(clientValidationError);
      return;
    }

    setIsLoading(true);

    try {
      const payload = {
        localSalesCount: Number(formData.localSalesCount),
        foreignSalesCount: Number(formData.foreignSalesCount),
        averageSaleAmount: Number(formData.averageSaleAmount)
      };

      const res = await fetch(`${API_BASE}/Commision`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (!res.ok) {
        const err = await res.json().catch(() => ({ error: res.statusText }));
        throw new Error(err.error || res.statusText || 'API error');
      }

      const data = await res.json();
      setResults(data);
    } catch (err) {
      setError(err.message || 'Unknown error');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <div className="logo-container">
          <img src={logo} className="App-logo" alt="Avalpha Technologies Logo" />
          <h1 className="company-title">Avalpha Technologies</h1>
          <h2 className="app-subtitle">Commission Calculator</h2>
        </div>
      </header>

      <main className="main-content">
        <div className="calculator-container">
          <div className="form-section">
            <h3>Sales Information</h3>
            <form onSubmit={handleSubmit} className="calculator-form">
              <div className="form-group">
                <label htmlFor="localSalesCount">Local Sales Count</label>
                <input
                  type="number"
                  id="localSalesCount"
                  name="localSalesCount"
                  value={formData.localSalesCount}
                  onChange={handleInputChange}
                  placeholder="Enter number of local sales"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="foreignSalesCount">Foreign Sales Count</label>
                <input
                  type="number"
                  id="foreignSalesCount"
                  name="foreignSalesCount"
                  value={formData.foreignSalesCount}
                  onChange={handleInputChange}
                  placeholder="Enter number of foreign sales"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="averageSaleAmount">Average Sale Amount (£)</label>
                <input
                  type="number"
                  step="0.01"
                  id="averageSaleAmount"
                  name="averageSaleAmount"
                  value={formData.averageSaleAmount}
                  onChange={handleInputChange}
                  placeholder="Enter average sale amount"
                  required
                />
              </div>

              {error && <div style={{ color: 'crimson', fontWeight: 600 }}>{error}</div>}

              <button
                type="submit"
                className={`calculate-btn ${isLoading ? 'loading' : ''}`}
                disabled={isLoading}
              >
                {isLoading ? 'Calculating...' : 'Calculate Commission'}
              </button>
            </form>
          </div>

          <div className="results-section">
            <h3>Commission Results</h3>

            {!results && <p>No results yet. Submit the form to calculate.</p>}

            {results && (
              <div className="results-grid">
                <div className="result-card avalpha-card">
                  <div className="result-header">
                    <h4>Avalpha Technologies</h4>
                    <span className="commission-rates">Local: 20% | Foreign: 35%</span>
                  </div>
                  <div className="result-amount">
                    {currencyFormatter.format(results.avalphaTechnologiesCommissionAmount ?? results.AvalphaTechnologiesCommissionAmount ?? results.avalphaTechnologiesCommission)}
                  </div>
                  <div>
                    <small>Local: {currencyFormatter.format(results.avalphaTechnologiesLocal ?? results.AvalphaTechnologiesLocal)}</small><br />
                    <small>Foreign: {currencyFormatter.format(results.avalphaTechnologiesForeign ?? results.AvalphaTechnologiesForeign)}</small>
                  </div>
                </div>

                <div className="result-card competitor-card">
                  <div className="result-header">
                    <h4>Competitor</h4>
                    <span className="commission-rates">Local: 2% | Foreign: 7.55%</span>
                  </div>
                  <div className="result-amount">
                    {currencyFormatter.format(results.competitorCommissionAmount ?? results.CompetitorCommissionAmount ?? results.competitorCommission)}
                  </div>
                  <div>
                    <small>Local: {currencyFormatter.format(results.competitorLocal ?? results.CompetitorLocal)}</small><br />
                    <small>Foreign: {currencyFormatter.format(results.competitorForeign ?? results.CompetitorForeign)}</small>
                  </div>
                </div>

                <div className="advantage-indicator">
                  <p className="advantage-text">
                    Avalpha Technologies advantage:&nbsp;
                    <strong>
                      {currencyFormatter.format(
                        (results.avalphaTechnologiesCommissionAmount ?? results.AvalphaTechnologiesCommissionAmount) -
                        (results.competitorCommissionAmount ?? results.CompetitorCommissionAmount)
                      )}
                    </strong>
                  </p>
                </div>
              </div>
            )}
          </div>
        </div>
      </main>

      <footer className="App-footer">
        <p>&copy; 2025 Avalpha Technologies. All rights reserved.</p>
      </footer>
    </div>
  );
}

export default App;
