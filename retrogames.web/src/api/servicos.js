import api from './api'

const dados = (resposta) => resposta.data.data

export const jogoApi = {
  listar: (filtros) => api.get('/Jogo', { params: filtros }).then(dados),
  buscarPorId: (id) => api.get(`/Jogo/${id}`).then(dados),
  inserir: (jogo) => api.post('/Jogo', jogo).then(dados),
  alterar: (id, jogo) => api.put(`/Jogo/${id}`, jogo).then(dados),
  desativar: (id) => api.delete(`/Jogo/${id}`).then(dados),
}

export const plataformaApi = {
  listar: () => api.get('/Plataforma').then(dados),
  inserir: (plataforma) => api.post('/Plataforma', plataforma).then(dados),
  alterar: (id, plataforma) => api.put(`/Plataforma/${id}`, plataforma).then(dados),
  excluir: (id) => api.delete(`/Plataforma/${id}`).then(dados),
}

export const generoApi = {
  listar: () => api.get('/Genero').then(dados),
}

export const autenticacaoApi = {
  entrar: (email, senha) => api.post('/Autenticacao/login', { email, senha }).then(dados),
  sair: () => api.post('/Autenticacao/logout'),
  usuarioAtual: () => api.get('/Autenticacao/eu').then(dados),
}

export const condicaoApi = {
  listar: () => api.get('/CondicaoJogo').then(dados),
}
