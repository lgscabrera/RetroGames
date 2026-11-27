import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos'
import { useCarrinho } from '../../hooks/useCarrinho'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../utils/formatar'

export default function Carrinho() {
  const { itens, alterarQuantidade, remover, limpar, total } = useCarrinho()
  const navigate = useNavigate()
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const finalizar = async () => {
    setEnviando(true)
    setErro('')
    try {
      await pedidoApi.finalizar(itens.map((item) => ({
        idJogo: item.idJogo,
        quantidade: item.quantidade,
      })))
      limpar()
      navigate('/meus-pedidos')
    } catch (falha) {
      setErro(obterMensagemErro(falha))
      setEnviando(false)
    }
  }

  if (itens.length === 0) {
    return (
      <div>
        <h1 className="h3 mb-3">Carrinho</h1>
        <p className="text-muted">Seu carrinho está vazio.</p>
        <Link className="btn btn-primary" to="/">Ir ao catálogo</Link>
      </div>
    )
  }

  return (
    <div>
      <h1 className="h3 mb-3">Carrinho</h1>
      {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
      <div className="table-responsive">
        <table className="table align-middle">
          <thead>
            <tr><th>Jogo</th><th>Preço</th><th>Quantidade</th><th>Subtotal</th><th></th></tr>
          </thead>
          <tbody>
            {itens.map((item) => (
              <tr key={item.idJogo}>
                <td>{item.nome}</td>
                <td>{formatarPreco(item.preco)}</td>
                <td>
                  <input type="number" min="1" max="10" className="form-control"
                    style={{ width: '6rem' }} value={item.quantidade}
                    onChange={(evento) => alterarQuantidade(item.idJogo, evento.target.value)} />
                </td>
                <td>{formatarPreco(item.preco * item.quantidade)}</td>
                <td>
                  <button type="button" className="btn btn-outline-danger btn-sm"
                    onClick={() => remover(item.idJogo)}>Remover</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="d-flex justify-content-end align-items-center gap-3">
        <strong>Total: {formatarPreco(total)}</strong>
        <button type="button" className="btn btn-primary" onClick={finalizar} disabled={enviando}>
          {enviando ? 'Finalizando...' : 'Finalizar compra'}
        </button>
      </div>
    </div>
  )
}
