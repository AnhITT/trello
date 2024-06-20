import Box from '@mui/material/Box'
import Container from '@mui/material/Container'

function BoardContent() {
  return (
    <Container disableGutters maxWidth="false" sx={{ height: '100vh' }}>
      <Box
        sx={{
          backgroundColor: 'primary.main',
          width: '100%',
          height: theme =>
            `calc(100vh - ${theme.trello.appBarHeight} - ${theme.trello.boardBarHeight})`,
          display: 'flex',
          alignItems: 'center'
        }}
      >
        Board content
      </Box>
    </Container>
  )
}

export default BoardContent
