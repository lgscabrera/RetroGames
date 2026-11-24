import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <div>
      <h1 className="h3">Página não encontrada</h1>
      <Link to="/">Voltar ao catálogo</Link>
    </div>
  )
}
