import { Link, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <>
      <nav className="navbar navbar-dark bg-dark mb-4">
        <div className="container">
          <Link className="navbar-brand" to="/">RetroGames</Link>
        </div>
      </nav>
      <main className="container">
        <Outlet />
      </main>
      <footer className="container text-muted small py-4">
        RetroGames
      </footer>
    </>
  )
}
