import {
  Box,
  Button,
  Checkbox,
  Divider,
  FormControl,
  Link,
  TextField,
  Typography,
  Container,
  FormControlLabel,
  Card,
  Backdrop
} from '@mui/material'
import GoogleIcon from '@mui/icons-material/Google'
import TrelloIcon from '~/assets/trello.svg?react'
import SvgIcon from '@mui/material/SvgIcon'
import { NavLink as RouterLink } from 'react-router-dom'
import Background from '~/assets/images/background-auth.jpg'

function Login() {
  return (
    <>
      <Backdrop
        open
        sx={{
          zIndex: 0,
          color: '#fff',
          backgroundImage: `url(${Background})`,
          filter: 'blur(8px)'
        }}
      />
      <Box
        sx={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center'
        }}
      >
        <Card sx={{ maxWidth: '80%', p: 4 }}>
          <Container
            component="main"
            maxWidth="sm"
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center'
            }}
          >
            <Box
              component={RouterLink}
              to="/board"
              sx={{
                display: 'flex',
                justifyContent: 'space-between',
                width: '100%',
                mb: 4,
                cursor: 'pointer',
                textDecoration: 'none'
              }}
            >
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <SvgIcon
                  component={TrelloIcon}
                  sx={{ width: 60, height: 60, mr: 1, color: '#1975d1' }}
                />
                <Typography
                  variant="h3"
                  component="div"
                  sx={{
                    fontSize: '2.5rem',
                    fontWeight: 'bold',
                    color: '#1975d1'
                  }}
                >
                  Trello
                </Typography>
              </Box>
            </Box>

            <Typography component="h1" variant="h4">
              Sign in
            </Typography>
            <Typography variant="body2" align="center">
              Do not have an account?{' '}
              <Link href="#replace-with-a-link">Sign up!</Link>
            </Typography>

            <Box component="form" sx={{ mt: 3 }}>
              <FormControl fullWidth sx={{ mt: 2 }} required>
                <TextField
                  label="Email"
                  type="text"
                  variant="outlined"
                  autoFocus
                />
              </FormControl>
              <FormControl fullWidth sx={{ mt: 2 }} required>
                <TextField label="Password" type="text" variant="outlined" />
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
                    control={<Checkbox defaultChecked />}
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
              >
                Sign in
              </Button>
            </Box>
            <Button
              variant="outlined"
              fullWidth
              startIcon={<GoogleIcon />}
              sx={{ mt: 3, mb: 2, width: '100%', padding: '12px 16px' }}
            >
              Continue with Google
            </Button>
            <Box sx={{ mt: 5 }}>
              <Typography variant="body2" align="center">
                © Trello {new Date().getFullYear()}
              </Typography>
            </Box>
          </Container>
        </Card>
      </Box>
    </>
  )
}

export default Login
