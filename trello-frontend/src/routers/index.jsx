import { lazy } from 'react'

/****Layouts*****/
const BoardLayout = lazy(() => import('~/components/Layout/BoardLayout'))
/***** Pages ****/

const Board = lazy(() => import('~/pages/Boards/_id'))
const NotFound = lazy(() => import('~/pages/NotFound/NotFound'))
const Workspaces = lazy(() => import('~/pages/Workspaces'))
const Login = lazy(() => import('~/pages/Auth/login'))

/*****Routes******/

const ThemeRoutes = [
  {
    path: '/',
    element: <Login />
  },
  {
    path: '/',
    element: <BoardLayout />,
    children: [
      { path: '/workspace', element: <Workspaces /> },
      { path: '/not-found', element: <NotFound /> },
      { path: '/board', element: <Board /> }
    ]
  }
]

export default ThemeRoutes
