import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import Avatar from '@mui/material/Avatar'
import ListItemText from '@mui/material/ListItemText'
import TextField from '@mui/material/TextField'
import InputAdornment from '@mui/material/InputAdornment'
import SearchIcon from '@mui/icons-material/Search'
import CloseIcon from '@mui/icons-material/Close'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'

const MessageList = ({
  me,
  searchText,
  setSearchText,
  chats,
  handleUserClick,
  selectedChat
}) => {
  const formatDate = date => {
    const d = new Date(date)
    const day = d.getDate().toString().padStart(2, '0')
    const month = (d.getMonth() + 1).toString().padStart(2, '0') // Months are zero-based
    const year = d.getFullYear()
    const hours = d.getHours().toString().padStart(2, '0')
    const minutes = d.getMinutes().toString().padStart(2, '0')

    return `${hours}:${minutes} ${day}-${month}-${year}`
  }

  return (
    <>
      <TextField
        id="outlined-search"
        type="text"
        value={searchText}
        placeholder="Search..."
        size="medium"
        onChange={e => setSearchText(e.target.value)}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon />
            </InputAdornment>
          ),
          endAdornment: searchText && (
            <CloseIcon
              fontSize="small"
              sx={{ cursor: 'pointer' }}
              onClick={() => setSearchText('')}
            />
          )
        }}
        sx={{
          paddingX: '10px',
          width: '100%',
          margin: 'auto',
          marginTop: '10px'
        }}
      />
      <List sx={{ paddingX: '10px', flexGrow: 1, overflowY: 'auto' }}>
        {chats.map(chat => {
          const lastMessage = chat.messages[chat.messages.length - 1]

          // Kiểm tra xem có phải đoạn chat nhóm hay không
          const isGroupChat = chat.isGroup
          let displayName = ''
          let avatarUrl =
            'https://cdn.iconscout.com/icon/free/png-256/free-avatar-370-456322.png'

          if (isGroupChat) {
            displayName =
              chat.nameGroup ||
              chat.members
                .filter(member => member.id !== me.Id)
                .map(member => member.lastName)
                .join(', ')
            avatarUrl =
              chat.avatarGroup ||
              'https://www.shareicon.net/data/512x512/2016/06/30/788858_group_512x512.png'
          } else {
            const otherMember = chat.members.find(member => member.id !== me.Id)
            displayName = otherMember?.lastName || 'Unknown'
            avatarUrl = otherMember?.avatarUrl || avatarUrl
          }

          return (
            <ListItem
              key={chat.id}
              button
              onClick={() => handleUserClick(chat)}
              sx={{
                display: 'flex',
                alignItems: 'center',
                marginBottom: '1px',
                borderRadius: '10px',
                backgroundColor:
                  selectedChat?.id === chat.id
                    ? 'rgba(0, 0, 0, 0.2)'
                    : 'transparent',
                '&:hover': {
                  backgroundColor: 'rgba(0, 0, 0, 0.2)'
                }
              }}
            >
              <Avatar
                sx={{ width: 60, height: 60, marginRight: '10px' }}
                src={avatarUrl}
              />
              <div style={{ flexGrow: 1 }}>
                <ListItemText
                  primary={displayName}
                  primaryTypographyProps={{
                    fontWeight: 'bold'
                  }}
                  secondary={
                    lastMessage ? (
                      <Box
                        sx={{
                          position: 'relative',
                          display: 'flex',
                          alignItems: 'center'
                        }}
                      >
                        <Typography
                          variant="body2"
                          color="textSecondary"
                          sx={{ flexGrow: 1, fontSize: '1rem' }}
                        >
                          {lastMessage.text}
                        </Typography>
                        <Typography
                          variant="caption"
                          color="textSecondary"
                          sx={{ position: 'absolute', right: '0' }}
                        >
                          {formatDate(lastMessage.createdDate)}
                        </Typography>
                      </Box>
                    ) : (
                      'No messages yet'
                    )
                  }
                />
              </div>
            </ListItem>
          )
        })}
      </List>
    </>
  )
}

export default MessageList
