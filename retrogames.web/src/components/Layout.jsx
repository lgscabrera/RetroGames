import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { useCarrinho } from '../hooks/useCarrinho'

export default function Layout() {
  const { usuario, estaAutenticado, ehAdministrador, sair } = useAutenticacao()
  const navigate = useNavigate()
  const { quantidadeTotal } = useCarrinho()

  const aoSair = async () => {
    await sair()
    navigate('/')
  }

  return (
    <>
      <nav className="navbar navbar-expand navbar-dark bg-dark mb-4">
        <div className="container">
          <Link className="navbar-brand" to="/">RetroGames</Link>

          <ul className="navbar-nav me-auto">
            {ehAdministrador && (
              <li className="nav-item dropdown">
                <button type="button" className="nav-link dropdown-toggle btn btn-link"
                  data-bs-toggle="dropdown" aria-expanded="false">
                  Administração
                </button>
                <ul className="dropdown-menu">
                  <li><Link className="dropdown-item" to="/admin/jogos">Jogos</Link></li>
                  <li><Link className="dropdown-item" to="/admin/plataformas">Plataformas</Link></li>
                </ul>
              </li>
            )}
          </ul>

          <div className="d-flex align-items-center gap-3">
            {estaAutenticado && !ehAdministrador && (
              <>
                <Link className="text-light text-decoration-none" to="/carrinho">
                  Carrinho ({quantidadeTotal})
                </Link>
                <Link className="text-light text-decoration-none" to="/meus-pedidos">Meus pedidos</Link>
              </>
            )}
            {estaAutenticado ? (
              <>
                <span className="navbar-text text-light">Olá, {usuario.nome}</span>
                <button type="button" className="btn btn-outline-light btn-sm" onClick={aoSair}>
                  Sair
                </button>
              </>
            ) : (
              <Link className="btn btn-outline-light btn-sm" to="/login">Entrar</Link>
            )}
          </div>
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
