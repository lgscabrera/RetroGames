import { Link, Outlet } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'

export default function Layout() {
  const { usuario } = useAutenticacao()

  return (
    <>
      <nav className="navbar navbar-dark bg-dark mb-4">
        <div className="container">
          <Link className="navbar-brand" to="/">RetroGames</Link>
          <span className="navbar-text text-light">
            {usuario ? usuario.nome : 'visitante'}
          </span>
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
