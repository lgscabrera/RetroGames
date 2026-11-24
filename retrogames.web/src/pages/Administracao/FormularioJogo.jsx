import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { condicaoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'

const formularioVazio = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: [],
}

export default function FormularioJogo() {
  const { id } = useParams()
  const navigate = useNavigate()

  const [formulario, setFormulario] = useState(formularioVazio)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [erroCarga, setErroCarga] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [erroGeral, setErroGeral] = useState('')
  const [salvando, setSalvando] = useState(false)

  useEffect(() => {
    let cancelado = false
    setCarregando(true)
    setErroCarga('')

    const pedidos = [plataformaApi.listar(), generoApi.listar(), condicaoApi.listar()]
    if (id) pedidos.push(jogoApi.buscarPorId(id))

    Promise.all(pedidos)
      .then(([listaPlataformas, listaGeneros, listaCondicoes, jogo]) => {
        if (cancelado) return
        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
        setCondicoes(listaCondicoes)

        if (jogo) {
          const plataforma = listaPlataformas.find(
            (p) => p.id === jogo.idPlataforma || p.nome === jogo.nomePlataforma
          )
          const condicao = listaCondicoes.find(
            (c) =>
              c.id === jogo.idCondicaoJogo ||
              (c.descricao ?? c.nome) === jogo.descricaoCondicao
          )
          const idsGeneros = (jogo.generos ?? [])
            .map((g) =>
              typeof g === 'string'
                ? listaGeneros.find((x) => x.nome === g)?.id
                : g.id
            )
            .filter((x) => x !== undefined)

          setFormulario({
            nome: jogo.nome ?? '',
            descricao: jogo.descricao ?? '',
            anoLancamento: String(jogo.anoLancamento ?? ''),
            preco: String(jogo.preco ?? ''),
            estoque: String(jogo.estoque ?? ''),
            idPlataforma: plataforma ? String(plataforma.id) : '',
            idCondicaoJogo: condicao ? String(condicao.id) : '',
            idsGeneros,
          })
        }
      })
      .catch((falha) => {
        if (!cancelado) setErroCarga(obterMensagemErro(falha))
      })
      .finally(() => {
        if (!cancelado) setCarregando(false)
      })

    return () => {
      cancelado = true
    }
  }, [id])

  const alterarCampo = (campo, valor) => {
    setFormulario((atual) => ({ ...atual, [campo]: valor }))
  }

  const alternarGenero = (idGenero) => {
    setFormulario((atual) => {
      const marcado = atual.idsGeneros.includes(idGenero)
      return {
        ...atual,
        idsGeneros: marcado
          ? atual.idsGeneros.filter((g) => g !== idGenero)
          : [...atual.idsGeneros, idGenero],
      }
    })
  }

  const salvar = async (evento) => {
    evento.preventDefault()
    setSalvando(true)
    setErrosCampos({})
    setErroGeral('')

    const corpo = {
      nome: formulario.nome,
      descricao: formulario.descricao || null,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
    }

    try {
      if (id) await jogoApi.alterar(id, corpo)
      else await jogoApi.inserir(corpo)
      navigate('/admin/jogos')
    } catch (falha) {
      const campos = obterErrosDosCampos(falha)
      if (Object.keys(campos).length > 0) setErrosCampos(campos)
      else setErroGeral(obterMensagemErro(falha))
      setSalvando(false)
    }
  }

  const classeCampo = (campo, base = 'form-control') =>
    `${base}${errosCampos[campo] ? ' is-invalid' : ''}`

  const erroDoCampo = (campo) =>
    errosCampos[campo] && (
      <div className="invalid-feedback d-block" role="alert">{errosCampos[campo]}</div>
    )

  if (carregando) return <Carregando texto="Carregando formulário..." />
  if (erroCarga) return <MensagemErro mensagem={erroCarga} />

  return (
    <div>
      <h1 className="h3 mb-3">{id ? 'Editar jogo' : 'Novo jogo'}</h1>

      {erroGeral && <div className="alert alert-danger" role="alert">{erroGeral}</div>}

      <form className="row g-3" onSubmit={salvar} noValidate>
        <div className="col-12 col-md-8">
          <label htmlFor="nome" className="form-label">Nome</label>
          <input id="nome" className={classeCampo('nome')} value={formulario.nome}
            onChange={(e) => alterarCampo('nome', e.target.value)} />
          {erroDoCampo('nome')}
        </div>
        <div className="col-12 col-md-4">
          <label htmlFor="ano" className="form-label">Ano de lançamento</label>
          <input id="ano" type="number" className={classeCampo('anoLancamento')}
            value={formulario.anoLancamento}
            onChange={(e) => alterarCampo('anoLancamento', e.target.value)} />
          {erroDoCampo('anoLancamento')}
        </div>

        <div className="col-12">
          <label htmlFor="descricao" className="form-label">Descrição</label>
          <textarea id="descricao" rows="3" className={classeCampo('descricao')}
            value={formulario.descricao}
            onChange={(e) => alterarCampo('descricao', e.target.value)} />
          {erroDoCampo('descricao')}
        </div>

        <div className="col-6 col-md-3">
          <label htmlFor="preco" className="form-label">Preço</label>
          <input id="preco" type="number" step="0.01" className={classeCampo('preco')}
            value={formulario.preco}
            onChange={(e) => alterarCampo('preco', e.target.value)} />
          {erroDoCampo('preco')}
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="estoque" className="form-label">Estoque</label>
          <input id="estoque" type="number" className={classeCampo('estoque')}
            value={formulario.estoque}
            onChange={(e) => alterarCampo('estoque', e.target.value)} />
          {erroDoCampo('estoque')}
        </div>
        <div className="col-12 col-md-3">
          <label htmlFor="plataforma" className="form-label">Plataforma</label>
          <select id="plataforma" className={classeCampo('idPlataforma', 'form-select')}
            value={formulario.idPlataforma}
            onChange={(e) => alterarCampo('idPlataforma', e.target.value)}>
            <option value="">Selecione</option>
            {plataformas.map((p) => <option key={p.id} value={p.id}>{p.nome}</option>)}
          </select>
          {erroDoCampo('idPlataforma')}
        </div>
        <div className="col-12 col-md-3">
          <label htmlFor="condicao" className="form-label">Condição</label>
          <select id="condicao" className={classeCampo('idCondicaoJogo', 'form-select')}
            value={formulario.idCondicaoJogo}
            onChange={(e) => alterarCampo('idCondicaoJogo', e.target.value)}>
            <option value="">Selecione</option>
            {condicoes.map((c) => (
              <option key={c.id} value={c.id}>{c.descricao ?? c.nome}</option>
            ))}
          </select>
          {erroDoCampo('idCondicaoJogo')}
        </div>

        <fieldset className="col-12">
          <legend className="form-label fs-6">Gêneros</legend>
          <div className="d-flex flex-wrap gap-3">
            {generos.map((g) => (
              <div className="form-check" key={g.id}>
                <input id={`genero-${g.id}`} type="checkbox" className="form-check-input"
                  checked={formulario.idsGeneros.includes(g.id)}
                  onChange={() => alternarGenero(g.id)} />
                <label htmlFor={`genero-${g.id}`} className="form-check-label">{g.nome}</label>
              </div>
            ))}
          </div>
          {erroDoCampo('idsGeneros')}
        </fieldset>

        <div className="col-12 d-flex gap-2">
          <button type="submit" className="btn btn-primary" disabled={salvando}>
            {salvando ? 'Salvando...' : 'Salvar'}
          </button>
          <Link className="btn btn-outline-secondary" to="/admin/jogos">Cancelar</Link>
        </div>
      </form>
    </div>
  )
}
