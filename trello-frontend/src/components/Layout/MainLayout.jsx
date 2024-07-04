import { Outlet } from 'react-router-dom'

const DefaultLayout = () => {
  return (
    <div>
      <p>main</p>
      <Outlet />
    </div>
  )
}

export default DefaultLayout
