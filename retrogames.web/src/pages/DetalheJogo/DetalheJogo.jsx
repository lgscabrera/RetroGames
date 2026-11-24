import { useCallback, useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../utils/formatar'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado'

const nomeDoGenero = (genero) => (typeof genero === 'string' ? genero : genero.nome)

export default function DetalheJogo() {
  const { id } = useParams()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [naoEncontrado, setNaoEncontrado] = useState(false)
  const [erro, setErro] = useState('')

  const carregarJogo = useCallback(() => {
    setCarregando(true)
    setErro('')
    setNaoEncontrado(false)
    jogoApi
      .buscarPorId(id)
      .then(setJogo)
      .catch((falha) => {
        if (falha?.response?.status === 404) setNaoEncontrado(true)
        else setErro(obterMensagemErro(falha))
      })
      .finally(() => setCarregando(false))
  }, [id])

  useEffect(() => {
    carregarJogo()
  }, [carregarJogo])

  return (
    <div>
      <Link to="/" className="d-inline-block mb-3">← Voltar ao catálogo</Link>

      {carregando && <Carregando texto="Carregando jogo..." />}
      <MensagemErro mensagem={erro} aoTentarNovamente={carregarJogo} />

      {!carregando && naoEncontrado && <EstadoVazio titulo="Jogo não encontrado." />}

      {!carregando && jogo && (
        <article>
          <h1 className="h3">{jogo.nome}</h1>
          <p className="text-muted">
            {jogo.nomePlataforma} · {jogo.anoLancamento}
            {jogo.descricaoCondicao ? ` · ${jogo.descricaoCondicao}` : ''}
          </p>
          <p>{jogo.descricao || 'Sem descrição.'}</p>
          {jogo.generos?.length > 0 && (
            <p className="small">Gêneros: {jogo.generos.map(nomeDoGenero).join(', ')}</p>
          )}
          <p className="h4">{formatarPreco(jogo.preco)}</p>
          {jogo.estoque === 0
            ? <span className="badge text-bg-secondary">Esgotado</span>
            : <span className="small text-muted">{jogo.estoque} em estoque</span>}
        </article>
      )}
    </div>
  )
}
