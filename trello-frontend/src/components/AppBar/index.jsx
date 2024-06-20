import Box from '@mui/material/Box'
import Container from '@mui/material/Container'
import ModeSelect from '../ModeSelect'

function AppBar() {
  return (
    <Container disableGutters maxWidth="false" sx={{ height: '100vh' }}>
      <Box
        sx={{
          backgroundColor: 'primary.light',
          width: '100%',
          height: theme => theme.trello.appBarHeight,
          display: 'flex',
          alignItems: 'center'
        }}
      >
        <ModeSelect />
      </Box>
    </Container>
  )
}

export default AppBar
