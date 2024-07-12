import React, { useEffect, useState } from 'react'
import * as signalR from '@microsoft/signalr'
import Grid from '@mui/material/Grid'
import Paper from '@mui/material/Paper'
import List from '@mui/material/List'
import Box from '@mui/material/Box'
import ListItem from '@mui/material/ListItem'
import ListItemText from '@mui/material/ListItemText'
import TextField from '@mui/material/TextField'
import Avatar from '@mui/material/Avatar'
import SearchIcon from '@mui/icons-material/Search'
import CloseIcon from '@mui/icons-material/Close'
import { InputAdornment } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import AttachFileIcon from '@mui/icons-material/AttachFile'
import CallIcon from '@mui/icons-material/Call'
import VideoCallIcon from '@mui/icons-material/VideoCall'
import InfoIcon from '@mui/icons-material/Info'
import ImageIcon from '@mui/icons-material/Image'
import GifBoxIcon from '@mui/icons-material/GifBox'
import AddReactionIcon from '@mui/icons-material/AddReaction'
import SendIcon from '@mui/icons-material/Send'
import { GetAllUserAPI } from '~/apis/User'
//aes c#
const Chat = () => {
  const [searchText, setSearchText] = useState('')
  const [connection, setConnection] = useState(null)
  const [messages, setMessages] = useState([])
  const [message, setMessage] = useState('')
  const [users, setUsers] = useState([])
  const [selectedUser, setSelectedUser] = useState(null)

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7181/chatHub', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build()

    setConnection(newConnection)
  }, [])

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(result => {
          console.log('Connected!')

          connection.on('ReceiveMessage', (user, message) => {
            const updatedMessages = [...messages]
            updatedMessages.push({ user, message })
            setMessages(updatedMessages)
          })
        })
        .catch(e => console.log('Connection failed: ', e))
    }
  }, [connection, messages])

  useEffect(() => {
    // Gọi API để lấy danh sách người dùng khi component được mount
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

  const sendMessage = async () => {
    if (connection) {
      try {
        await connection.send('SendMessage', message)
        setMessage('')
      } catch (e) {
        console.log(e)
      }
    } else {
      alert('No connection to server yet.')
    }
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
              {users.map((user, index) => (
                <ListItem
                  key={user.id}
                  button
                  onClick={() => setSelectedUser(user)}
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
                      'https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=0IgNVRfVQHwQ7kNvgHHHET1&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYA7RGpLXIPL38mDqVJNkRPtbr407y8gvQfoxv-2C4VWsw&oe=6692CD7D'
                    }
                  />
                  <div style={{ flexGrow: 1 }}>
                    <ListItemText
                      primary={user.lastName}
                      secondary={`Last message: ${
                        messages.length > 0
                          ? messages[messages.length - 1].message
                          : ''
                      }`}
                    />
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
              overflowY: 'auto',
              position: 'relative'
            }}
          >
            {selectedUser && (
              <div
                style={{
                  padding: '10px',
                  backgroundColor: '#f5f5f5',
                  borderBottom: '1px solid #ddd',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between' // Để icon được căn phải
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center' }}>
                  <Avatar
                    sx={{ width: 40, height: 40, marginRight: '10px' }}
                    src={
                      selectedUser.avatarUrl ||
                      'https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=0IgNVRfVQHwQ7kNvgHHHET1&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYA7RGpLXIPL38mDqVJNkRPtbr407y8gvQfoxv-2C4VWsw&oe=6692CD7D'
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
            <List>
              {messages.map((msg, index) => (
                <ListItem key={index}>
                  <ListItemText primary={`${msg.user}: ${msg.message}`} />
                </ListItem>
              ))}
            </List>
            <div
              style={{
                position: 'absolute',
                bottom: 0,
                left: 0,
                width: '100%',
                display: 'flex',
                padding: '10px'
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
                onKeyPress={e => {
                  if (e.key === 'Enter') {
                    sendMessage()
                  }
                }}
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
                      onClick={() => setSearchText('')}
                    />
                  )
                }}
              />
              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center'
                }}
              >
                <SendIcon
                  sx={{ marginX: '15px', cursor: 'pointer' }}
                  onClick={sendMessage}
                />
              </Box>
            </div>
          </Paper>
        </Grid>
      </Grid>
    </div>
  )
}

export default Chat
