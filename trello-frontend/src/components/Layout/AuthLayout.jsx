import AppBar from '~/components/AppBar'

const AuthLayout = ({ children }) => {
  return (
    <div>
      <AppBar />
      {children}
    </div>
  )
}

export default AuthLayout
