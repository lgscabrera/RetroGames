import axios from 'axios'

const api = axios.create({ baseURL: '/api', withCredentials: true })

// 401 esperado: visitante (eu) ou senha errada (login). Os demais indicam sessão expirada.
const rotasSemAviso = ['/Autenticacao/eu', '/Autenticacao/login']

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const url = erro?.config?.url ?? ''
    const esperado = rotasSemAviso.some((rota) => url.includes(rota))
    if (erro?.response?.status === 401 && !esperado) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }
    return Promise.reject(erro)
  }
)

export default api
