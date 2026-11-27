import { useEffect, useState } from 'react'
import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../utils/formatar'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado'

export default function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  const carregar = () => {
    setCarregando(true)
    setErro('')
    pedidoApi.meus()
      .then(setPedidos)
      .catch((falha) => setErro(obterMensagemErro(falha)))
      .finally(() => setCarregando(false))
  }

  useEffect(() => {
    let cancelado = false
    pedidoApi.meus()
      .then((resultado) => { if (!cancelado) setPedidos(resultado) })
      .catch((falha) => { if (!cancelado) setErro(obterMensagemErro(falha)) })
      .finally(() => { if (!cancelado) setCarregando(false) })
    return () => { cancelado = true }
  }, [])

  return (
    <div>
      <h1 className="h3 mb-3">Meus pedidos</h1>
      {carregando && <Carregando texto="Carregando pedidos..." />}
      <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />
      {!carregando && !erro && pedidos.length === 0 &&
        <EstadoVazio titulo="Você ainda não fez pedidos." />}
      {!carregando && pedidos.map((pedido) => (
        <section key={pedido.id} className="border-bottom py-3">
          <div className="d-flex justify-content-between align-items-start mb-2">
            <div>
              <strong>Pedido #{pedido.id}</strong>
              <div className="text-muted small">
                {new Date(pedido.data).toLocaleString('pt-BR')}
              </div>
            </div>
            <span className="badge text-bg-secondary">{pedido.situacao}</span>
          </div>
          <ul className="mb-2">
            {pedido.itens.map((item) => (
              <li key={item.idJogo}>
                {item.nomeJogo} - {item.quantidade} x {formatarPreco(item.precoUnitario)}
              </li>
            ))}
          </ul>
          <strong>Total: {formatarPreco(pedido.total)}</strong>
        </section>
      ))}
    </div>
  )
}
