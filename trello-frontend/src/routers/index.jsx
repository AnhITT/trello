import { lazy } from 'react'
import { Outlet, Navigate } from 'react-router-dom'
import { useAuth } from '~/context/AuthProvider'
/****Layouts*****/
const BoardLayout = lazy(() => import('~/components/Layout/BoardLayout'))

/***** Pages ****/
const Board = lazy(() => import('~/pages/Boards/_id'))
const NotFound = lazy(() => import('~/pages/NotFound/NotFound'))
const Workspaces = lazy(() => import('~/pages/Workspaces'))
const Login = lazy(() => import('~/pages/Auth/index'))
const VerifyEmail = lazy(() => import('~/pages/Auth/VerifyEmail'))

/*****Routes******/
const PublicRoutes = [
  {
    element: null,
    children: [
      { path: '/', element: <Login /> },
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
    children: [{ path: '/board', element: <Board /> }]
  }
]

const PrivateRoute = () => {
  const { isAuthenticated } = useAuth()
  return isAuthenticated() ? <Outlet /> : <Navigate to="/" />
}

export { PublicRoutes, PrivateRoutes, PrivateRoute }
