import { Link } from 'react-router-dom'
import { formatarPreco } from '../utils/formatar'

export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <article className="card h-100">
      <div className="card-body d-flex flex-column">
        <h2 className="h6 card-title">{jogo.nome}</h2>
        <p className="card-text small text-muted mb-1">
          {jogo.nomePlataforma} · {jogo.anoLancamento} · {jogo.descricaoCondicao}
        </p>
        <p className="card-text small mb-2">{jogo.generos.join(', ')}</p>
        <div className="mt-auto">
          <strong>{formatarPreco(jogo.preco)}</strong>{' '}
          {esgotado && <span className="badge text-bg-secondary">Esgotado</span>}
          <div className="mt-2">
            <Link to={`/jogos/${jogo.id}`}>Ver detalhes</Link>
          </div>
        </div>
      </div>
    </article>
  )
}
