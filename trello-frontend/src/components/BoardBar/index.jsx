import Box from '@mui/material/Box'
import Container from '@mui/material/Container'

function BoardBar() {
  return (
    <Container disableGutters maxWidth="false" sx={{ height: '100vh' }}>
      <Box
        sx={{
          backgroundColor: 'primary.dark',
          width: '100%',
          height: theme => theme.trello.boardBarHeight,
          display: 'flex',
          alignItems: 'center'
        }}
      >
        Board bar
      </Box>
    </Container>
  )
}

export default BoardBar
