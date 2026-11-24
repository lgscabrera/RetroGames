import { useCallback, useEffect, useState } from 'react'
import { plataformaApi } from '../../api/servicos'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado'

const formularioVazio = { nome: '', fabricante: '', anoLancamento: '' }

export default function Plataforma() {
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')

  const [formulario, setFormulario] = useState(formularioVazio)
  const [emEdicao, setEmEdicao] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})
  const [erroGeral, setErroGeral] = useState('')
  const [salvando, setSalvando] = useState(false)

  const carregarLista = useCallback(() => {
    setCarregando(true)
    setErroLista('')
    plataformaApi
      .listar()
      .then(setPlataformas)
      .catch((falha) => setErroLista(obterMensagemErro(falha)))
      .finally(() => setCarregando(false))
  }, [])

  useEffect(() => {
    carregarLista()
  }, [carregarLista])

  const alterarCampo = (campo, valor) => {
    setFormulario((atual) => ({ ...atual, [campo]: valor }))
  }

  const limparFormulario = () => {
    setFormulario(formularioVazio)
    setEmEdicao(null)
    setErrosCampos({})
    setErroGeral('')
  }

  const iniciarEdicao = (plataforma) => {
    setEmEdicao(plataforma.id)
    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    })
    setErrosCampos({})
    setErroGeral('')
  }

  const salvar = async (evento) => {
    evento.preventDefault()
    setSalvando(true)
    setErrosCampos({})
    setErroGeral('')

    const corpo = {
      nome: formulario.nome,
      fabricante: formulario.fabricante,
      anoLancamento: Number(formulario.anoLancamento),
    }

    try {
      if (emEdicao) await plataformaApi.alterar(emEdicao, corpo)
      else await plataformaApi.inserir(corpo)
      limparFormulario()
      carregarLista()
    } catch (falha) {
      const campos = obterErrosDosCampos(falha)
      if (Object.keys(campos).length > 0) setErrosCampos(campos)
      else setErroGeral(obterMensagemErro(falha))
    } finally {
      setSalvando(false)
    }
  }

  const excluir = async (plataforma) => {
    if (!window.confirm(`Excluir a plataforma "${plataforma.nome}"?`)) return
    setErroGeral('')
    try {
      await plataformaApi.excluir(plataforma.id)
      if (emEdicao === plataforma.id) limparFormulario()
      carregarLista()
    } catch (falha) {
      setErroGeral(obterMensagemErro(falha))
    }
  }

  const classeCampo = (campo) => `form-control${errosCampos[campo] ? ' is-invalid' : ''}`

  return (
    <div>
      <h1 className="h3 mb-3">Plataformas</h1>

      {erroGeral && <div className="alert alert-danger" role="alert">{erroGeral}</div>}

      <form className="row g-3 mb-4" onSubmit={salvar} noValidate>
        <div className="col-12 col-md-4">
          <label htmlFor="nome" className="form-label">Nome</label>
          <input id="nome" className={classeCampo('nome')} value={formulario.nome}
            onChange={(e) => alterarCampo('nome', e.target.value)} />
          {errosCampos.nome && <div className="invalid-feedback" role="alert">{errosCampos.nome}</div>}
        </div>
        <div className="col-12 col-md-4">
          <label htmlFor="fabricante" className="form-label">Fabricante</label>
          <input id="fabricante" className={classeCampo('fabricante')} value={formulario.fabricante}
            onChange={(e) => alterarCampo('fabricante', e.target.value)} />
          {errosCampos.fabricante && <div className="invalid-feedback" role="alert">{errosCampos.fabricante}</div>}
        </div>
        <div className="col-6 col-md-2">
          <label htmlFor="ano" className="form-label">Ano</label>
          <input id="ano" type="number" className={classeCampo('anoLancamento')}
            value={formulario.anoLancamento}
            onChange={(e) => alterarCampo('anoLancamento', e.target.value)} />
          {errosCampos.anoLancamento && (
            <div className="invalid-feedback" role="alert">{errosCampos.anoLancamento}</div>
          )}
        </div>
        <div className="col-6 col-md-2 d-flex align-items-end gap-2">
          <button type="submit" className="btn btn-primary" disabled={salvando}>
            {emEdicao ? 'Salvar' : 'Incluir'}
          </button>
          {emEdicao && (
            <button type="button" className="btn btn-outline-secondary" onClick={limparFormulario}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      {carregando && <Carregando texto="Carregando plataformas..." />}
      <MensagemErro mensagem={erroLista} aoTentarNovamente={carregarLista} />

      {!carregando && !erroLista && plataformas.length === 0 && (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada." />
      )}

      {!carregando && !erroLista && plataformas.length > 0 && (
        <div className="table-responsive">
          <table className="table align-middle">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano</th>
                <th className="text-end">Ações</th>
              </tr>
            </thead>
            <tbody>
              {plataformas.map((p) => (
                <tr key={p.id}>
                  <td>{p.nome}</td>
                  <td>{p.fabricante}</td>
                  <td>{p.anoLancamento}</td>
                  <td className="text-end">
                    <button type="button" className="btn btn-sm btn-outline-primary me-2"
                      onClick={() => iniciarEdicao(p)}>
                      Editar
                    </button>
                    <button type="button" className="btn btn-sm btn-outline-danger"
                      onClick={() => excluir(p)}>
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
