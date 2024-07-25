import TextField from '@mui/material/TextField'
import Box from '@mui/material/Box'
import AddReactionIcon from '@mui/icons-material/AddReaction'
import SendIcon from '@mui/icons-material/Send'
import AddIcon from '@mui/icons-material/Add'
import ImageIcon from '@mui/icons-material/Image'
import GifBoxIcon from '@mui/icons-material/GifBox'
import AttachFileIcon from '@mui/icons-material/AttachFile'
const MessageInput = ({ message, setMessage, handleSendMessage }) => (
  <div
    style={{
      display: 'flex',
      padding: '10px',
      borderTop: '1px solid rgb(196, 196, 196)',
      position: 'sticky',
      bottom: 0,
      zIndex: 1
    }}
  >
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center'
      }}
    >
      <AddIcon sx={{ marginRight: '5px', cursor: 'pointer' }} />
      <AttachFileIcon sx={{ marginRight: '5px', cursor: 'pointer' }} />
      <ImageIcon sx={{ marginRight: '5px', cursor: 'pointer' }} />
      <GifBoxIcon sx={{ marginRight: '10px', cursor: 'pointer' }} />
    </Box>
    <TextField
      variant="outlined"
      label="Message"
      fullWidth
      size="small"
      value={message}
      onChange={e => setMessage(e.target.value)}
      sx={{
        borderRadius: '20px'
      }}
      InputProps={{
        endAdornment: (
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center'
            }}
          >
            <AddReactionIcon sx={{ marginRight: '15px', cursor: 'pointer' }} />
            <SendIcon sx={{ cursor: 'pointer' }} onClick={handleSendMessage} />
          </Box>
        )
      }}
      onKeyPress={e => {
        if (e.key === 'Enter') {
          handleSendMessage()
        }
      }}
    />
  </div>
)

export default MessageInput
