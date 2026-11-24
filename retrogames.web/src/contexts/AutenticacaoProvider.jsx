import { useCallback, useEffect, useMemo, useState } from 'react'
import { autenticacaoApi } from '../api/servicos'
import { AutenticacaoContext } from './autenticacaoContext'

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    autenticacaoApi
      .usuarioAtual()
      .then(setUsuario)
      .catch(() => setUsuario(null))
      .finally(() => setCarregando(false))
  }, [])

  useEffect(() => {
    const limpar = () => setUsuario(null)
    window.addEventListener('sessao:expirada', limpar)
    return () => window.removeEventListener('sessao:expirada', limpar)
  }, [])

  const entrar = useCallback(async (email, senha) => {
    await autenticacaoApi.entrar(email, senha)
    setUsuario(await autenticacaoApi.usuarioAtual())
  }, [])

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair()
    } finally {
      setUsuario(null)
    }
  }, [])

  const valor = useMemo(
    () => ({
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.perfil === 'Administrador',
      entrar,
      sair,
    }),
    [usuario, carregando, entrar, sair]
  )

  return <AutenticacaoContext.Provider value={valor}>{children}</AutenticacaoContext.Provider>
}
