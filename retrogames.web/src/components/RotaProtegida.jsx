import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { Carregando } from './MensagensEstado'

export default function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao()
  const local = useLocation()

  if (carregando) return <Carregando texto="Verificando sessão..." />

  if (!estaAutenticado) {
    const voltarPara = encodeURIComponent(local.pathname + local.search)
    return <Navigate to={`/login?voltarPara=${voltarPara}`} replace />
  }

  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
