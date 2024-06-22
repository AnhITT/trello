import { Box, Button, Container, Typography } from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'
import NOT_FOUND from '~/assets/notfound.jpg'

const NotFound = () => (
  <Box sx={{ padding: 4 }} title="404 Page Not Found">
    <Container>
      <Box sx={{ maxWidth: 480, margin: 'auto', textAlign: 'center' }}>
        <Typography variant="h3" paragraph>
          Page not found
        </Typography>
        <Typography sx={{ color: 'text.secondary' }}>
          Cannot find this page
        </Typography>
        <Box
          component="img"
          src={NOT_FOUND}
          sx={{ height: 260, mx: 'auto', my: { xs: 2, sm: 5 } }}
        />
        <Button to="/" size="large" variant="contained" component={RouterLink}>
          Go to home page
        </Button>
      </Box>
    </Container>
  </Box>
)

export default NotFound
