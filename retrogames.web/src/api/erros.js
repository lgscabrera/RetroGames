export function obterMensagemErro(erro) {
  if (!erro?.response) return 'Não foi possível conectar ao servidor.'
  return erro.response.data?.errorMessage || 'Ocorreu um erro inesperado.'
}

// 400 com responseCode 3: transforma { campo: [mensagens] } em { campo: mensagem }
export function obterErrosDosCampos(erro) {
  const corpo = erro?.response?.data
  if (corpo?.responseCode !== 3 || !corpo.data) return {}

  const campos = {}
  for (const [campo, mensagens] of Object.entries(corpo.data)) {
    campos[campo] = Array.isArray(mensagens) ? mensagens[0] : String(mensagens)
  }
  return campos
}
