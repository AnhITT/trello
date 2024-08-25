import { useState } from 'react'
import {
  Box,
  Button,
  Link,
  FormControl,
  TextField,
  Typography,
  Container,
  Card,
  CircularProgress,
  Alert
} from '@mui/material'
import TrelloIcon from '~/assets/svg/trello.svg?react'
import SvgIcon from '@mui/material/SvgIcon'
import { NavLink as RouterLink } from 'react-router-dom'
import { useAuth } from '~/context/AuthProvider'
import { Helmet } from 'react-helmet-async'

function ForgotPassword() {
  const auth = useAuth()
  const [otpRequest, setOtpRequest] = useState({
    email: '',
    otp: '',
    newPassword: ''
  })
  const [status, setStatus] = useState('')
  const [isOtpStage, setIsOtpStage] = useState(false)
  const [isSuccess, setIsSuccess] = useState(false)

  const handleChange = e => {
    const { name, value } = e.target
    setOtpRequest(prevState => ({
      ...prevState,
      [name]: value
    }))
  }

  const handleSubmitEmail = async e => {
    e.preventDefault()
    setStatus('')
    try {
      const response = await auth.forgotPassword(otpRequest.email)
      if (response) {
        setStatus(response)
      } else {
        setIsOtpStage(true)
      }
    } catch (error) {
      setStatus('Send info Forgot Password Failed')
    }
  }

  const handleConfirm = async e => {
    e.preventDefault()
    setStatus('')
    try {
      const response = await auth.confirmOTPChangePassword(otpRequest)
      if (response) {
        setStatus(response)
      } else {
        setIsSuccess(true)
      }
    } catch (error) {
      setStatus('OTP confirmation failed')
    }
  }

  const renderEmailForm = () => (
    <FormControl fullWidth required>
      <TextField
        label="Email"
        type="email"
        name="email"
        variant="standard"
        sx={{ mt: 2 }}
        value={otpRequest.email}
        onChange={handleChange}
        required
        InputLabelProps={{ required: false }}
      />
    </FormControl>
  )

  const renderOtpAndPasswordForm = () => (
    <>
      <FormControl fullWidth required>
        <TextField
          label="OTP"
          type="text"
          name="otp"
          sx={{ mt: 2 }}
          variant="standard"
          value={otpRequest.otp}
          onChange={handleChange}
          required
          InputLabelProps={{ required: false }}
        />
      </FormControl>
      <FormControl fullWidth required>
        <TextField
          label="New Password"
          type="password"
          name="newPassword"
          variant="standard"
          sx={{ mt: 2 }}
          value={otpRequest.newPassword}
          onChange={handleChange}
          required
          InputLabelProps={{ required: false }}
        />
      </FormControl>
    </>
  )

  const renderSuccessMessage = () => (
    <Box sx={{ mt: 3 }}>
      <Alert severity="success" sx={{ width: '100%', mb: 2 }}>
        Password changed successfully!
      </Alert>
      <Button
        fullWidth
        variant="contained"
        sx={{ padding: '12px 16px' }}
        component={RouterLink}
        to="/"
      >
        Go to Login
      </Button>
    </Box>
  )

  const renderForm = () => {
    if (isOtpStage) {
      if (!isSuccess) {
        return (
          <>
            {renderOtpAndPasswordForm()}
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, padding: '12px 16px' }}
              disabled={auth.loading}
            >
              {auth.loading ? <CircularProgress size={24} /> : 'Confirm'}
            </Button>
          </>
        )
      } else {
        return renderSuccessMessage()
      }
    } else {
      return (
        <>
          {!isSuccess && renderEmailForm()}
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, padding: '12px 16px' }}
            disabled={auth.loading}
          >
            {auth.loading ? <CircularProgress size={24} /> : 'Submit'}
          </Button>
        </>
      )
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
        <Card sx={{ p: 5, borderRadius: '20px' }}>
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
                ></Typography>
              </Box>
            </Box>
            <Helmet>
              <title>Trello - Forgot Password</title>
            </Helmet>
            <Typography
              component="h1"
              variant="h4"
              sx={{
                fontWeight: 'bold',
                color: 'primary.main'
              }}
            >
              Forgot Password
            </Typography>

            <Box
              component="form"
              onSubmit={isOtpStage ? handleConfirm : handleSubmitEmail}
            >
              {isOtpStage && !isSuccess && (
                <Alert severity="success" sx={{ width: '100%', mt: 1 }}>
                  OTP reset password sent to your email!
                </Alert>
              )}

              {status && (
                <Alert severity="error" sx={{ width: '100%', mt: 2 }}>
                  {status}
                </Alert>
              )}

              {renderForm()}

              <Typography variant="body2" align="center" sx={{ mt: 1 }}>
                Already have an account?{' '}
                <Link component={RouterLink} to="/">
                  Login!
                </Link>
              </Typography>
            </Box>
          </Container>
        </Card>
      </Box>
    </>
  )
}

export default ForgotPassword
