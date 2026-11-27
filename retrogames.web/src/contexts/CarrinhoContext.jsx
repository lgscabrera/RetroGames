import { useEffect, useState } from 'react'
import { CarrinhoContext } from './carrinhoContext'

const CHAVE = 'retrogames.carrinho'

function lerItens() {
  try {
    const salvo = JSON.parse(localStorage.getItem(CHAVE))
    return Array.isArray(salvo) ? salvo : []
  } catch {
    return []
  }
}

export function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(lerItens)

  useEffect(() => {
    localStorage.setItem(CHAVE, JSON.stringify(itens))
  }, [itens])

  const adicionar = (jogo) => {
    if (!jogo.ativo || jogo.estoque < 1) return
    setItens((atuais) => {
      const existente = atuais.find((item) => item.idJogo === jogo.id)
      if (existente) {
        return atuais.map((item) => item.idJogo === jogo.id
          ? { ...item, quantidade: Math.min(10, item.quantidade + 1) }
          : item)
      }
      return [...atuais, { idJogo: jogo.id, nome: jogo.nome, preco: jogo.preco, quantidade: 1 }]
    })
  }

  const alterarQuantidade = (idJogo, quantidade) => {
    const limitada = Math.min(10, Math.max(1, Number(quantidade) || 1))
    setItens((atuais) => atuais.map((item) =>
      item.idJogo === idJogo ? { ...item, quantidade: limitada } : item))
  }

  const remover = (idJogo) =>
    setItens((atuais) => atuais.filter((item) => item.idJogo !== idJogo))

  const limpar = () => setItens([])
  const total = itens.reduce((soma, item) => soma + item.preco * item.quantidade, 0)
  const quantidadeTotal = itens.reduce((soma, item) => soma + item.quantidade, 0)

  return (
    <CarrinhoContext.Provider value={{ itens, adicionar, alterarQuantidade, remover, limpar, total, quantidadeTotal }}>
      {children}
    </CarrinhoContext.Provider>
  )
}
