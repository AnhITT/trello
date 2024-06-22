import DefaultLayout from '~/components/Layout/DefaultLayout'

import Board from '~/pages/Boards/_id'
import NotFound from '~/pages/NotFound/NotFound'
import Workspaces from '~/pages/Workspaces'

export const publicRoutes = [
  { path: '/', element: Board, layout: null },
  { path: '/workspace', element: Workspaces, layout: DefaultLayout },
  { path: '/not-found', element: NotFound, layout: DefaultLayout }
]

export const privateRoutes = []
