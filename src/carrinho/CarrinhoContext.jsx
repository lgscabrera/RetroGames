import { createContext, useContext, useEffect, useState } from 'react'

const CHAVE = 'retrogames.carrinho'
const CarrinhoContext = createContext(null)

function lerSalvo() {
  try {
    const salvo = JSON.parse(localStorage.getItem(CHAVE))
    return Array.isArray(salvo) ? salvo : []
  } catch {
    return []
  }
}

const limitar = (quantidade) => Math.min(10, Math.max(1, quantidade))

export function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(lerSalvo)

  useEffect(() => {
    try {
      localStorage.setItem(CHAVE, JSON.stringify(itens))
    } catch { /* sem armazenamento: o carrinho vale só nesta aba */ }
  }, [itens])

  const adicionar = (jogo) =>
    setItens((atual) => {
      const existente = atual.find((i) => i.idJogo === jogo.id)
      if (existente) {
        return atual.map((i) =>
          i.idJogo === jogo.id ? { ...i, quantidade: limitar(i.quantidade + 1) } : i)
      }
      return [...atual, { idJogo: jogo.id, nome: jogo.nome, preco: jogo.preco, quantidade: 1 }]
    })

  const alterarQuantidade = (idJogo, quantidade) =>
    setItens((atual) =>
      atual.map((i) => (i.idJogo === idJogo ? { ...i, quantidade: limitar(quantidade) } : i)))

  const remover = (idJogo) => setItens((atual) => atual.filter((i) => i.idJogo !== idJogo))
  const limpar = () => setItens([])

  const total = itens.reduce((soma, i) => soma + i.preco * i.quantidade, 0)
  const quantidadeTotal = itens.reduce((soma, i) => soma + i.quantidade, 0)

  return (
    <CarrinhoContext.Provider
      value={{ itens, adicionar, alterarQuantidade, remover, limpar, total, quantidadeTotal }}>
      {children}
    </CarrinhoContext.Provider>
  )
}

export function useCarrinho() {
  return useContext(CarrinhoContext)
}
