import api from './api'

const dados = (resposta) => resposta.data.data

export const jogoApi = {
  listar: (filtros) => api.get('/Jogo', { params: filtros }).then(dados),
  buscarPorId: (id) => api.get(`/Jogo/${id}`).then(dados),
}

export const plataformaApi = {
  listar: () => api.get('/Plataforma').then(dados),
}

export const generoApi = {
  listar: () => api.get('/Genero').then(dados),
}

export const autenticacaoApi = {
  entrar: (email, senha) => api.post('/Autenticacao/login', { email, senha }).then(dados),
  sair: () => api.post('/Autenticacao/logout'),
  usuarioAtual: () => api.get('/Autenticacao/eu').then(dados),
}
