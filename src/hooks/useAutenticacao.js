import { useContext } from 'react'
import { AutenticacaoContext } from '../contexts/autenticacaoContext'

export function useAutenticacao() {
  return useContext(AutenticacaoContext)
}
