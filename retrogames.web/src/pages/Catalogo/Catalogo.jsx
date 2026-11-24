import { useCallback, useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { jogoApi, plataformaApi, generoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado'

export default function Catalogo() {
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const pagina = Number(parametros.get('pagina') ?? 1)

  const [textoBusca, setTextoBusca] = useState(busca)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  useEffect(() => {
    plataformaApi.listar().then(setPlataformas).catch(() => {})
    generoApi.listar().then(setGeneros).catch(() => {})
  }, [])

  const carregarJogos = useCallback(() => {
    setCarregando(true)
    setErro('')
    jogoApi
      .listar({
        busca: busca || undefined,
        idPlataforma: idPlataforma || undefined,
        idGenero: idGenero || undefined,
        pagina,
        tamanhoPagina: 12,
      })
      .then(setResultado)
      .catch((falha) => setErro(obterMensagemErro(falha)))
      .finally(() => setCarregando(false))
  }, [busca, idPlataforma, idGenero, pagina])

  useEffect(() => {
    carregarJogos()
  }, [carregarJogos])

  const alterarFiltro = (nome, valor) => {
    const novos = new URLSearchParams(parametros)
    if (valor) novos.set(nome, valor)
    else novos.delete(nome)
    if (nome !== 'pagina') novos.delete('pagina')
    setParametros(novos)
  }

  const enviarBusca = (evento) => {
    evento.preventDefault()
    alterarFiltro('busca', textoBusca.trim())
  }

  return (
    <div>
      <h1 className="h3 mb-3">Catálogo</h1>

      <form className="row g-2 mb-4" onSubmit={enviarBusca}>
        <div className="col-12 col-md-5">
          <label htmlFor="busca" className="form-label visually-hidden">Buscar jogo</label>
          <input id="busca" className="form-control" placeholder="Buscar pelo nome"
            value={textoBusca} onChange={(e) => setTextoBusca(e.target.value)} />
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="plataforma" className="form-label visually-hidden">Plataforma</label>
          <select id="plataforma" className="form-select" value={idPlataforma}
            onChange={(e) => alterarFiltro('idPlataforma', e.target.value)}>
            <option value="">Todas as plataformas</option>
            {plataformas.map((p) => <option key={p.id} value={p.id}>{p.nome}</option>)}
          </select>
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="genero" className="form-label visually-hidden">Gênero</label>
          <select id="genero" className="form-select" value={idGenero}
            onChange={(e) => alterarFiltro('idGenero', e.target.value)}>
            <option value="">Todos os gêneros</option>
            {generos.map((g) => <option key={g.id} value={g.id}>{g.nome}</option>)}
          </select>
        </div>
        <div className="col-12 col-md-1">
          <button type="submit" className="btn btn-primary w-100">Buscar</button>
        </div>
      </form>

      {carregando && <Carregando texto="Carregando jogos..." />}
      <MensagemErro mensagem={erro} aoTentarNovamente={carregarJogos} />

      {!carregando && !erro && resultado?.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado." />
      )}

      {!carregando && !erro && resultado?.itens.length > 0 && (
        <>
          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-4 g-3">
            {resultado.itens.map((jogo) => (
              <div className="col" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>
          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas}
            aoMudar={(p) => alterarFiltro('pagina', String(p))} />
        </>
      )}
    </div>
  )
}
