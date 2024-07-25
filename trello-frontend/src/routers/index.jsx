import { lazy } from 'react'
import { Outlet, Navigate } from 'react-router-dom'
import { useAuth } from '~/context/AuthProvider'
/****Layouts*****/
const BoardLayout = lazy(() => import('~/components/Layout/BoardLayout'))
const MainLayout = lazy(() => import('~/components/Layout/MainLayout'))

/***** Pages ****/
const Board = lazy(() => import('~/pages/Boards/_id'))
const NotFound = lazy(() => import('~/pages/NotFound/NotFound'))
const Workspaces = lazy(() => import('~/pages/Workspaces'))
const Login = lazy(() => import('~/pages/Auth/Login.jsx'))
const Register = lazy(() => import('~/pages/Auth/Register.jsx'))
const VerifyEmail = lazy(() => import('~/pages/Auth/VerifyEmail'))
const Chat = lazy(() => import('~/pages/Chat'))

/*****Routes******/
const PublicRoutes = [
  {
    element: <MainLayout />,
    children: [
      { path: '/', element: <Login /> },
      { path: '/register', element: <Register /> },
      { path: '/verify/register', element: <VerifyEmail /> }
    ]
  },
  {
    element: <BoardLayout />,
    children: [
      { path: '/workspace', element: <Workspaces /> },
      { path: '/not-found', element: <NotFound /> }
    ]
  }
]

const PrivateRoutes = [
  {
    element: <BoardLayout />,
    children: [
      { path: '/board', element: <Board /> },
      { path: '/chat', element: <Chat /> }
    ]
  }
]

const PrivateRoute = () => {
  const { isAuthenticated } = useAuth()
  return isAuthenticated() ? <Outlet /> : <Navigate to="/" />
}

export { PublicRoutes, PrivateRoutes, PrivateRoute }
