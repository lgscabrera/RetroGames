import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAutenticacao } from '../../hooks/useAutenticacao'
import { obterMensagemErro } from '../../api/erros'

export default function Login() {
  const { entrar } = useAutenticacao()
  const navigate = useNavigate()
  const [parametros] = useSearchParams()

  const destino = parametros.get('voltarPara') ?? ''
  const voltarPara = destino.startsWith('/') && !destino.startsWith('//') ? destino : '/'

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')

  const enviar = async (evento) => {
    evento.preventDefault()
    setEnviando(true)
    setErro('')
    try {
      await entrar(email.trim(), senha)
      navigate(voltarPara, { replace: true })
    } catch (falha) {
      setErro(obterMensagemErro(falha))
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-md-6 col-lg-4">
        <h1 className="h3 mb-3">Entrar</h1>

        {erro && <div className="alert alert-danger" role="alert">{erro}</div>}

        <form onSubmit={enviar}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">E-mail</label>
            <input id="email" type="email" className="form-control" autoComplete="username"
              value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">Senha</label>
            <input id="senha" type="password" className="form-control" autoComplete="current-password"
              value={senha} onChange={(e) => setSenha(e.target.value)} required />
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  )
}
