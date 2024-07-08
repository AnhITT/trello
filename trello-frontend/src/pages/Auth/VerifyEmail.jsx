import { useState, useEffect } from 'react'
import { useAuth } from '~/context/AuthProvider'
import { NavLink as RouterLink } from 'react-router-dom'
import { Box, Typography, Container, Card, Button } from '@mui/material'
import TrelloIcon from '~/assets/trello.svg?react'
import SvgIcon from '@mui/material/SvgIcon'

const VerifyEmailSuccess = () => {
  const auth = useAuth()
  const [content, setContent] = useState('')

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search)
    const code = urlParams.get('code')
    if (code) {
      const fetchContent = async () => {
        try {
          const response = await auth.verifyEmail(code)
          setContent(response)
        } catch (error) {
          setContent('Failed to verify email')
        }
      }
      fetchContent()
    }
  }, [auth])

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
        <Card sx={{ minWidth: '500px', p: 5, borderRadius: '20px' }}>
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
              to="/"
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

            <Typography
              component="h1"
              variant="h5"
              sx={{
                mt: 2,
                fontWeight: 'bold',
                color: 'primary.main'
              }}
            >
              {content}
            </Typography>
            <Button
              variant="outlined"
              fullWidth
              sx={{ mt: 2, mb: 2, width: '100%', padding: '12px 16px' }}
              component={RouterLink}
              to="/"
            >
              Login
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

export default VerifyEmailSuccess
