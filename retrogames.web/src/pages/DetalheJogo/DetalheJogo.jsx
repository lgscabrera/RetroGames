import { useParams } from 'react-router-dom'

export default function DetalheJogo() {
  const { id } = useParams()
  return <h1 className="h3">Jogo {id}</h1>
}
