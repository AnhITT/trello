import { Outlet } from 'react-router-dom'
import AppBar from '~/components/AppBar'
const BoardLayout = () => {
  return (
    <div>
      <AppBar />
      <Outlet />
    </div>
  )
}

export default BoardLayout
