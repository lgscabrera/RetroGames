import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAutenticacao } from '../../hooks/useAutenticacao'
import { obterMensagemErro } from '../../api/erros'

export default function Cadastro() {
  const { cadastrar } = useAutenticacao()
  const navigate = useNavigate()
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const enviar = async (evento) => {
    evento.preventDefault()
    setEnviando(true)
    setErro('')
    try {
      await cadastrar(nome.trim(), email.trim(), senha)
      navigate('/', { replace: true })
    } catch (falha) {
      setErro(obterMensagemErro(falha))
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-md-6 col-lg-4">
        <h1 className="h3 mb-3">Cadastrar cliente</h1>
        {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
        <form onSubmit={enviar}>
          <div className="mb-3">
            <label htmlFor="nome" className="form-label">Nome</label>
            <input id="nome" className="form-control" value={nome}
              onChange={(evento) => setNome(evento.target.value)} required maxLength={100} />
          </div>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">E-mail</label>
            <input id="email" type="email" className="form-control" value={email}
              onChange={(evento) => setEmail(evento.target.value)} required maxLength={256} />
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">Senha</label>
            <input id="senha" type="password" className="form-control" value={senha}
              onChange={(evento) => setSenha(evento.target.value)} required minLength={8} maxLength={100} />
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
            {enviando ? 'Cadastrando...' : 'Cadastrar'}
          </button>
        </form>
      </div>
    </div>
  )
}
