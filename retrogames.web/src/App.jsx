import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/DetalheJogo/DetalheJogo'
import Login from './pages/Login/Login'
import NaoEncontrada from './pages/NaoEncontrada/NaoEncontrada'

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
