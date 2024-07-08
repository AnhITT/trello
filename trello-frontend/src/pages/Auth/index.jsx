import { useState } from 'react'
import {
  Box,
  Button,
  Checkbox,
  FormControl,
  Link,
  TextField,
  Typography,
  Container,
  FormControlLabel,
  Card,
  CircularProgress,
  Alert
} from '@mui/material'
import GoogleIcon from '@mui/icons-material/Google'
import TrelloIcon from '~/assets/svg/trello.svg?react'
import SvgIcon from '@mui/material/SvgIcon'
import { NavLink as RouterLink } from 'react-router-dom'
import { useAuth } from '~/context/AuthProvider'
import { Helmet } from 'react-helmet-async'

function Auth() {
  const auth = useAuth()
  const [isSignUp, setIsSignUp] = useState(false)
  const [loginRequest, setLoginRequest] = useState({
    username: '',
    password: ''
  })
  const [registerRequest, setRegisterRequest] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: ''
  })
  const [loginError, setLoginError] = useState('')
  const [registerError, setRegisterError] = useState('')

  const handleLoginChange = e => {
    const { name, value } = e.target
    setLoginRequest(prev => ({ ...prev, [name]: value }))
  }

  const handleRegisterChange = e => {
    const { name, value } = e.target
    setRegisterRequest(prev => ({ ...prev, [name]: value }))
  }

  const handleLoginSubmit = async e => {
    e.preventDefault()
    setLoginError('')
    try {
      const response = await auth.login(loginRequest)
      if (response) {
        setLoginError(response)
      }
    } catch (error) {
      setLoginError('Login Failed')
    }
  }

  const handleRegisterSubmit = async e => {
    e.preventDefault()
    setRegisterError('')
    try {
      const response = await auth.register(registerRequest)
      if (response) {
        setRegisterError(response)
      }
    } catch (error) {
      setRegisterError('Register Failed')
    }
  }

  return (
    <>
      <Box
        sx={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          bgcolor: theme =>
            theme.palette.mode === 'dark' ? '#34495e' : '#1976d2',
          width: '100%'
        }}
      >
        <Card sx={{ maxWidth: '80%', p: 5, borderRadius: '20px' }}>
          <Container
            component="main"
            maxWidth="sm"
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              height: '100%'
            }}
          >
            <Box
              component={RouterLink}
              to="/board"
              sx={{
                display: 'flex',
                justifyContent: 'center',
                width: '100%',
                mb: 2,
                cursor: 'pointer',
                textDecoration: 'none'
              }}
            >
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <SvgIcon
                  component={TrelloIcon}
                  sx={{ width: 60, height: 60, mr: 1, color: 'primary.main' }}
                />
                <Typography
                  variant="h3"
                  component="div"
                  sx={{
                    fontSize: '2.5rem',
                    fontWeight: 'bold',
                    color: 'primary.main'
                  }}
                >
                  Trello
                </Typography>
              </Box>
            </Box>
            {isSignUp ? (
              <>
                <Helmet>
                  <title>Trello - Register</title>
                </Helmet>
                <Typography
                  component="h1"
                  variant="h4"
                  sx={{
                    mt: 2,
                    fontWeight: 'bold',
                    color: 'primary.main'
                  }}
                >
                  Register
                </Typography>

                <Box component="form" onSubmit={handleRegisterSubmit}>
                  {registerError && (
                    <Alert severity="error" sx={{ width: '100%', mt: 2 }}>
                      {registerError}
                    </Alert>
                  )}
                  <FormControl sx={{ minWidth: '310px' }} required>
                    <TextField
                      fullWidth
                      label="First Name"
                      type="text"
                      variant="standard"
                      name="firstName"
                      sx={{ mt: 2 }}
                      value={registerRequest.firstName}
                      onChange={handleRegisterChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                    <TextField
                      label="Last Name"
                      type="text"
                      variant="standard"
                      name="lastName"
                      sx={{ mt: 2 }}
                      value={registerRequest.lastName}
                      onChange={handleRegisterChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                    <TextField
                      label="Email"
                      type="text"
                      variant="standard"
                      name="email"
                      sx={{ mt: 2 }}
                      value={registerRequest.email}
                      onChange={handleRegisterChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                    <TextField
                      label="Number Phone"
                      type="text"
                      variant="standard"
                      name="phoneNumber"
                      sx={{ mt: 2 }}
                      value={registerRequest.phoneNumber}
                      onChange={handleRegisterChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                  </FormControl>
                  <Box
                    sx={{
                      display: 'flex',
                      justifyContent: 'space-between',
                      alignItems: 'center',
                      mt: 2
                    }}
                  ></Box>
                  <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{ mt: 3, padding: '12px 16px' }}
                    disabled={auth.loading}
                  >
                    {auth.loading ? <CircularProgress size={24} /> : 'Register'}
                  </Button>
                  <Typography variant="body2" align="center" sx={{ mt: 1 }}>
                    Already have an account?{' '}
                    <Link href="#" onClick={() => setIsSignUp(false)}>
                      Login!
                    </Link>
                  </Typography>
                </Box>
              </>
            ) : (
              <>
                <Helmet>
                  <title>Trello - Login</title>
                </Helmet>
                <Typography
                  component="h1"
                  variant="h4"
                  sx={{
                    mt: 2,

                    fontWeight: 'bold',
                    color: 'primary.main'
                  }}
                >
                  Login
                </Typography>

                <Box component="form" onSubmit={handleLoginSubmit}>
                  {loginError && (
                    <Alert severity="error" sx={{ width: '100%', mt: 2 }}>
                      {loginError}
                    </Alert>
                  )}
                  <FormControl fullWidth required>
                    <TextField
                      label="Email"
                      type="text"
                      variant="standard"
                      name="username"
                      sx={{ mt: 2 }}
                      value={loginRequest.username}
                      onChange={handleLoginChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                    <TextField
                      label="Password"
                      type="password"
                      variant="standard"
                      name="password"
                      sx={{ mt: 2 }}
                      value={loginRequest.password}
                      onChange={handleLoginChange}
                      required
                      InputLabelProps={{ required: false }}
                    />
                  </FormControl>

                  <Box
                    sx={{
                      display: 'flex',
                      justifyContent: 'space-between',
                      alignItems: 'center',
                      mt: 2
                    }}
                  >
                    <FormControl>
                      <FormControlLabel
                        control={<Checkbox />}
                        label="Remember me"
                      />
                    </FormControl>
                    <Link href="/">Forgot your password?</Link>
                  </Box>
                  <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{ mt: 3, padding: '12px 16px' }}
                    disabled={auth.loading}
                  >
                    {auth.loading ? <CircularProgress size={24} /> : 'Login'}
                  </Button>
                </Box>
                <Button
                  variant="outlined"
                  fullWidth
                  startIcon={<GoogleIcon />}
                  sx={{ mt: 2, mb: 2, width: '100%', padding: '12px 16px' }}
                >
                  Continue with Google
                </Button>
                <Typography variant="body2" align="center">
                  Do not have an account?{' '}
                  <Link href="#" onClick={() => setIsSignUp(true)}>
                    Register!
                  </Link>
                </Typography>
              </>
            )}
          </Container>
        </Card>
      </Box>
    </>
  )
}

export default Auth
