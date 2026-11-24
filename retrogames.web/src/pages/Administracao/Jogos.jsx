import { useCallback, useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../utils/formatar'
import Paginacao from '../../components/Paginacao'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado'

export default function Jogos() {
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const pagina = Number(parametros.get('pagina') ?? 1)

  const [textoBusca, setTextoBusca] = useState(busca)
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [erroAcao, setErroAcao] = useState('')

  const carregarJogos = useCallback(() => {
    setCarregando(true)
    setErro('')
    jogoApi
      .listar({
        busca: busca || undefined,
        pagina,
        tamanhoPagina: 10,
        incluirInativos: true,
      })
      .then(setResultado)
      .catch((falha) => setErro(obterMensagemErro(falha)))
      .finally(() => setCarregando(false))
  }, [busca, pagina])

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

  const desativar = async (jogo) => {
    if (!window.confirm(`Desativar o jogo "${jogo.nome}"?`)) return
    setErroAcao('')
    try {
      await jogoApi.desativar(jogo.id)
      carregarJogos()
    } catch (falha) {
      setErroAcao(obterMensagemErro(falha))
    }
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="h3 mb-0">Jogos</h1>
        <Link className="btn btn-primary" to="/admin/jogos/novo">Novo jogo</Link>
      </div>

      <form className="row g-2 mb-3" onSubmit={enviarBusca}>
        <div className="col-12 col-md-6">
          <label htmlFor="busca" className="form-label visually-hidden">Buscar jogo</label>
          <input id="busca" className="form-control" placeholder="Buscar pelo nome"
            value={textoBusca} onChange={(e) => setTextoBusca(e.target.value)} />
        </div>
        <div className="col-12 col-md-2">
          <button type="submit" className="btn btn-outline-primary w-100">Buscar</button>
        </div>
      </form>

      {erroAcao && <div className="alert alert-danger" role="alert">{erroAcao}</div>}

      {carregando && <Carregando texto="Carregando jogos..." />}
      <MensagemErro mensagem={erro} aoTentarNovamente={carregarJogos} />

      {!carregando && !erro && resultado?.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado." />
      )}

      {!carregando && !erro && resultado?.itens.length > 0 && (
        <>
          <div className="table-responsive">
            <table className="table align-middle">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Situação</th>
                  <th className="text-end">Ações</th>
                </tr>
              </thead>
              <tbody>
                {resultado.itens.map((j) => (
                  <tr key={j.id}>
                    <td>{j.nome}</td>
                    <td>{j.nomePlataforma}</td>
                    <td>{formatarPreco(j.preco)}</td>
                    <td>{j.estoque}</td>
                    <td>
                      <span className={`badge ${j.ativo ? 'text-bg-success' : 'text-bg-secondary'}`}>
                        {j.ativo ? 'Ativo' : 'Inativo'}
                      </span>
                    </td>
                    <td className="text-end">
                      <Link className="btn btn-sm btn-outline-primary me-2"
                        to={`/admin/jogos/${j.id}/editar`}>
                        Editar
                      </Link>
                      {j.ativo && (
                        <button type="button" className="btn btn-sm btn-outline-danger"
                          onClick={() => desativar(j)}>
                          Desativar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas}
            aoMudar={(p) => alterarFiltro('pagina', String(p))} />
        </>
      )}
    </div>
  )
}
