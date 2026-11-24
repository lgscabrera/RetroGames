export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null

  return (
    <nav aria-label="Paginação" className="d-flex justify-content-center align-items-center gap-3 my-4">
      <button type="button" className="btn btn-outline-primary"
        disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>
        Anterior
      </button>
      <span>Página {pagina} de {totalPaginas}</span>
      <button type="button" className="btn btn-outline-primary"
        disabled={pagina >= totalPaginas} onClick={() => aoMudar(pagina + 1)}>
        Próxima
      </button>
    </nav>
  )
}
