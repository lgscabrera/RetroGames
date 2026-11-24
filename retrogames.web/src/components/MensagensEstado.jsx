export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="d-flex align-items-center gap-2 my-4" role="status">
      <div className="spinner-border spinner-border-sm" aria-hidden="true"></div>
      <span>{texto}</span>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  if (!mensagem) return null
  return (
    <div className="alert alert-danger d-flex justify-content-between align-items-center" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-sm btn-outline-danger" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ titulo }) {
  return <p className="text-muted my-4">{titulo}</p>
}
