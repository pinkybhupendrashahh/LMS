import { useEffect, useState } from "react";

type HealthStatus = {
  status: string;
  service: string;
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

function ApiStatus() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    fetch(`${apiBaseUrl}/api/health`, { signal: controller.signal })
      .then((response) => {
        if (!response.ok) throw new Error(`API returned HTTP ${response.status}`);
        return response.json() as Promise<HealthStatus>;
      })
      .then(setHealth)
      .catch((reason: unknown) => {
        if (reason instanceof Error && reason.name !== "AbortError") {
          setError(reason.message);
        }
      });
    return () => controller.abort();
  }, []);

  if (health) {
    return <span className="status status--online">{health.service} connected</span>;
  }
  if (error) {
    return <span className="status status--offline">API unavailable: {error}</span>;
  }
  return <span className="status">Connecting to API…</span>;
}

const phaseOneItems = [
  { label: "Solution structure", detail: "React + .NET 8 Clean Architecture", icon: "01" },
  { label: "Data model", detail: "23 SQL Server entity families", icon: "02" },
  { label: "Database setup", detail: "EF Core code-first migrations", icon: "03" },
];

export default function App() {
  return (
    <main className="shell">
      <aside className="sidebar">
        <div className="brand"><span className="brand-mark">L</span><span>Learnspace</span></div>
        <div className="nav-label">WORKSPACE</div>
        <a className="nav-item nav-item--active" href="#overview"><span>▦</span> Overview</a>
        <div className="nav-label nav-label--lower">PROJECT</div>
        <div className="phase-link"><span className="phase-dot" /> Phase 1 <span className="phase-state">IN PROGRESS</span></div>
        <div className="sidebar-footer">Learning starts here.</div>
      </aside>

      <section className="main-panel" id="overview">
        <header className="topbar"><span>Workspace <span className="crumb">/</span> Overview</span><span className="avatar">L</span></header>
        <div className="content">
          <div className="eyebrow">LEARNING MANAGEMENT SYSTEM</div>
          <h1>Your learning platform,<br /><span>built step by step.</span></h1>
          <p className="intro">The foundation is in place. This workspace tracks the Phase 1 setup and verifies the API connection.</p>
          <div className="health-card"><span className="pulse" /><div><strong>Development environment</strong><small>Live service check</small></div><ApiStatus /></div>
          <div className="section-heading"><div><h2>Phase 1 foundation</h2><p>Project structure, data model, and database setup</p></div><span className="progress-label">01 / 01</span></div>
          <div className="checklist">
            {phaseOneItems.map((item) => (
              <article className="check-item" key={item.label}>
                <span className="item-icon">{item.icon}</span>
                <div><h3>{item.label}</h3><p>{item.detail}</p></div>
                <span className="checkmark" aria-label="included">✓</span>
              </article>
            ))}
          </div>
          <footer>PHASE 1 <span>·</span> FOUNDATION</footer>
        </div>
      </section>
    </main>
  );
}
