import { useEffect, useState, useRef } from 'react'
import Grid from '@mui/material/Grid'
import Paper from '@mui/material/Paper'
import List from '@mui/material/List'
import Box from '@mui/material/Box'
import ListItem from '@mui/material/ListItem'
import TextField from '@mui/material/TextField'
import Avatar from '@mui/material/Avatar'
import SearchIcon from '@mui/icons-material/Search'
import CloseIcon from '@mui/icons-material/Close'
import AddIcon from '@mui/icons-material/Add'
import AttachFileIcon from '@mui/icons-material/AttachFile'
import CallIcon from '@mui/icons-material/Call'
import VideoCallIcon from '@mui/icons-material/VideoCall'
import ListItemText from '@mui/material/ListItemText'
import InfoIcon from '@mui/icons-material/Info'
import ImageIcon from '@mui/icons-material/Image'
import GifBoxIcon from '@mui/icons-material/GifBox'
import AddReactionIcon from '@mui/icons-material/AddReaction'
import SendIcon from '@mui/icons-material/Send'
import { InputAdornment } from '@mui/material'
import { GetAllUserAPI } from '~/apis/User'
import { GetChatByMembers } from '~/apis/Chat'
import Message from '~/components/Message'
import { useAuth } from '~/context/AuthProvider'
import * as signalR from '@microsoft/signalr'
import Cookies from 'js-cookie'

const Chat = () => {
  const [searchText, setSearchText] = useState('')
  const [chat, setChat] = useState('')
  const [message, setMessage] = useState('')
  const [users, setUsers] = useState([])
  const [selectedUser, setSelectedUser] = useState(null)
  const [messages, setMessages] = useState([])
  const { user } = useAuth()
  const connection = useRef(null)
  const messagesContainerRef = useRef(null)

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await GetAllUserAPI()
        if (response.statusCode === 200) {
          setUsers(response.data)
        } else {
          console.log('Failed to fetch users:', response.message)
        }
      } catch (error) {
        console.log('Error fetching users:', error)
      }
    }

    fetchUsers()
  }, [])

  useEffect(() => {
    const token = Cookies.get('token')
    connection.current = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7181/chatHub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build()

    connection.current
      .start()
      .then(() => console.log('Connected to SignalR'))
      .catch(err => console.error('SignalR Connection Error: ', err))

    connection.current.on('ReceiveMessage', apiMessage => {
      setMessages(prevMessages => [...prevMessages, apiMessage])
    })

    return () => {
      connection.current.stop()
    }
  }, [])

  useEffect(() => {
    if (messagesContainerRef.current) {
      messagesContainerRef.current.scrollTop =
        messagesContainerRef.current.scrollHeight
    }
  }, [messages])

  const handleUserClick = async userFocus => {
    setSelectedUser(userFocus)
    try {
      const userIds = [user.Id, userFocus.id]
      const response = await GetChatByMembers(userIds)
      if (response.statusCode === 200) {
        setChat(response.data.id)
        setMessages(response.data.messages)
        console.log('value: ', response.data)
      } else {
        setMessages([])
      }
    } catch (error) {
      setMessages([])
    }
  }

  const handleSendMessage = async () => {
    if (message.trim() === '' || !selectedUser) return

    const apiMessage = {
      ChatId: chat,
      Sender: user.Id,
      Type: 'text',
      Text: message,
      File: null
    }

    try {
      await connection.current.invoke('SendMessage', apiMessage)
      setMessage('')
    } catch (error) {
      console.error('Error sending message: ', error)
    }
  }

  const getMessageProps = index => {
    const isFirstInGroup =
      index === 0 || messages[index].sender !== messages[index - 1].sender
    const isLastInGroup =
      index === messages.length - 1 ||
      messages[index].sender !== messages[index + 1].sender
    return { isFirstInGroup, isLastInGroup }
  }

  return (
    <div>
      <Grid container spacing={1} style={{ marginTop: '10px' }}>
        <Grid item xs={3}>
          <Paper
            elevation={3}
            style={{
              height: '85.5vh',
              overflowY: 'auto',
              display: 'flex',
              flexDirection: 'column'
            }}
          >
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
                    sx={{
                      cursor: 'pointer'
                    }}
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
              {users.map(user => (
                <ListItem
                  key={user.id}
                  button
                  onClick={() => handleUserClick(user)}
                  sx={{
                    display: 'flex',
                    alignItems: 'center',
                    marginBottom: '1px',
                    borderRadius: '10px',
                    backgroundColor:
                      selectedUser?.id === user.id
                        ? 'rgba(0, 0, 0, 0.1)'
                        : 'transparent',
                    '&:hover': {
                      backgroundColor: 'rgba(0, 0, 0, 0.1)'
                    }
                  }}
                >
                  <Avatar
                    sx={{ width: 60, height: 60, marginRight: '10px' }}
                    src={
                      user.avatarUrl ||
                      'https://cdn.iconscout.com/icon/free/png-256/free-avatar-370-456322.png'
                    }
                  />
                  <div style={{ flexGrow: 1 }}>
                    <ListItemText primary={user.lastName} />
                  </div>
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>
        <Grid item xs={8.9}>
          <Paper
            elevation={3}
            style={{
              height: '85.5vh',
              display: 'flex',
              flexDirection: 'column'
            }}
          >
            {selectedUser && (
              <div
                style={{
                  padding: '10px',
                  borderBottom: '1px solid #ddd',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between'
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center' }}>
                  <Avatar
                    sx={{ width: 40, height: 40, marginRight: '10px' }}
                    src={
                      selectedUser.avatarUrl ||
                      'https://cdn.iconscout.com/icon/free/png-256/free-avatar-370-456322.png'
                    }
                  />
                  <div>
                    <h4 style={{ margin: 0 }}>{selectedUser.lastName}</h4>
                    <p style={{ margin: 0, fontSize: '14px', color: '#666' }}>
                      Online
                    </p>
                  </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                  <CallIcon sx={{ marginRight: '10px', cursor: 'pointer' }} />
                  <VideoCallIcon
                    sx={{
                      marginRight: '10px',
                      cursor: 'pointer',
                      fontSize: '30px'
                    }}
                  />
                  <InfoIcon sx={{ marginRight: '10px', cursor: 'pointer' }} />
                </div>
              </div>
            )}
            <div
              style={{
                flexGrow: 1,
                display: 'flex',
                flexDirection: 'column',
                padding: '10px',
                overflowY: 'auto',
                overflowX: 'hidden'
              }}
              ref={messagesContainerRef}
            >
              {messages.length > 0 ? (
                <>
                  {messages.map((msg, index) => {
                    const { isFirstInGroup } = getMessageProps(index)
                    return (
                      <Message
                        key={index}
                        userId={user.Id}
                        sender={msg.sender}
                        text={msg.text}
                        senderAvatar={msg.senderAvatar}
                        isFirstInGroup={isFirstInGroup}
                        lastName={selectedUser.lastName}
                      />
                    )
                  })}
                </>
              ) : (
                <ListItem
                  disablePadding
                  sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    height: '100%'
                  }}
                >
                  <button
                    onClick={() => console.log('Start chat')}
                    style={{
                      padding: '10px',
                      borderRadius: '10px',
                      border: '1px solid #ddd',
                      backgroundColor: '#f5f5f5',
                      cursor: 'pointer'
                    }}
                  >
                    Start a conversation with {selectedUser?.lastName}
                  </button>
                </ListItem>
              )}
            </div>
            <div
              style={{
                display: 'flex',
                padding: '10px',
                borderTop: '1px solid #ddd',
                backgroundColor: 'white',
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
                <AttachFileIcon
                  sx={{ marginRight: '5px', cursor: 'pointer' }}
                />
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
                    <AddReactionIcon
                      fontSize="small"
                      sx={{
                        cursor: 'pointer'
                      }}
                    />
                  )
                }}
                onKeyPress={e => {
                  if (e.key === 'Enter') {
                    handleSendMessage()
                  }
                }}
              />
              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center'
                }}
                onClick={handleSendMessage}
              >
                <SendIcon sx={{ marginX: '15px', cursor: 'pointer' }} />
              </Box>
            </div>
          </Paper>
        </Grid>
      </Grid>
    </div>
  )
}

export default Chat
