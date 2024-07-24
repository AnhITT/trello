import Box from '@mui/material/Box'
import Paper from '@mui/material/Paper'
import Avatar from '@mui/material/Avatar'
import Typography from '@mui/material/Typography'

const Message = ({
  userId,
  sender,
  text,
  senderAvatar,
  isFirstInGroup,
  lastName
}) => {
  const isSender = userId === sender

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: isSender ? 'flex-end' : 'flex-start',
        marginBottom: '5px'
      }}
    >
      {isSender ? (
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row-reverse',
            alignItems: 'center',
            width: '100%'
          }}
        >
          <Paper
            sx={{
              padding: '10px',
              maxWidth: '100%',
              backgroundColor: '#1976d2',
              color: 'white',
              borderRadius: '25% 10%'
            }}
          >
            <Typography variant="body1" sx={{ wordWrap: 'break-word' }}>
              {text}
            </Typography>
          </Paper>
        </Box>
      ) : (
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            width: '100%'
          }}
        >
          {isFirstInGroup && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                marginRight: '8px'
              }}
            >
              <Avatar
                src={
                  senderAvatar ||
                  'https://cdn.iconscout.com/icon/free/png-256/free-avatar-370-456322.png'
                }
                alt="Avatar"
                sx={{ width: 32, height: 32 }}
              />
            </Box>
          )}
          <Paper
            sx={{
              padding: '10px',
              maxWidth: '100%',
              backgroundColor: '#1976d2',
              color: 'white',
              borderRadius: '25% 10%',
              marginLeft: isFirstInGroup ? '0px' : '40px'
            }}
          >
            <Typography variant="body1" sx={{ wordWrap: 'break-word' }}>
              {text}
            </Typography>
          </Paper>
        </Box>
      )}
    </Box>
  )
}

export default Message
