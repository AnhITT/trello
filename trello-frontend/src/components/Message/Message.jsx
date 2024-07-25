import Box from '@mui/material/Box'
import Paper from '@mui/material/Paper'
import Avatar from '@mui/material/Avatar'
import Typography from '@mui/material/Typography'
import Tooltip from '@mui/material/Tooltip'

const Message = ({
  me,
  sender,
  senderName,
  senderAvatar,
  text,
  time,
  isFirstInGroup
}) => {
  const isSender = me === sender

  const formatTime = time => {
    const date = new Date(time)
    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0') // Months are zero-based
    const year = date.getFullYear()
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')

    return `${hours}:${minutes} ${day}-${month}-${year}`
  }

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: isSender ? 'flex-end' : 'flex-start',
        marginBottom: '5px',
        position: 'relative'
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
          <Tooltip title={formatTime(time)} arrow>
            <Paper
              sx={{
                padding: '10px',
                maxWidth: '100%',
                backgroundColor: '#1976d2',
                color: 'white',
                borderRadius: '15px'
              }}
            >
              <Typography variant="body1" sx={{ wordWrap: 'break-word' }}>
                {text}
              </Typography>
            </Paper>
          </Tooltip>
        </Box>
      ) : (
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center'
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
          <Box sx={{ position: 'relative', width: '100%' }}>
            {isFirstInGroup && (
              <Typography
                variant="caption"
                sx={{
                  position: 'absolute',
                  top: '-20px',
                  width: '100%',
                  display: 'flex',
                  justifyContent: 'center'
                }}
              >
                {senderName}
              </Typography>
            )}
            <Tooltip title={formatTime(time)} arrow>
              <Paper
                sx={{
                  padding: '10px',
                  maxWidth: '100%',
                  backgroundColor: '#1976d2',
                  color: 'white',
                  borderRadius: '15px',
                  marginLeft: isFirstInGroup ? '0px' : '40px'
                }}
              >
                <Typography variant="body1" sx={{ wordWrap: 'break-word' }}>
                  {text}
                </Typography>
              </Paper>
            </Tooltip>
          </Box>
        </Box>
      )}
    </Box>
  )
}

export default Message
