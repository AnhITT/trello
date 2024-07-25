import { useEffect, useState, useRef } from 'react'
import Grid from '@mui/material/Grid'
import Paper from '@mui/material/Paper'
import { useAuth } from '~/context/AuthProvider'
import * as signalR from '@microsoft/signalr'
import Cookies from 'js-cookie'
import { GetChatByMe } from '~/apis/Chat'
import MessageList from '~/components/Message/MessageList'
import MessageInfo from '~/components/Message/MessageInfo'
import ChatBox from '~/components/Message/ChatBox'
import MessageInput from '~/components/Message/MessageInput'

const Chat = () => {
  const [searchText, setSearchText] = useState('')
  const [chats, setChats] = useState([])
  const [message, setMessage] = useState('')
  const [selectedChat, setSelectedChat] = useState(null)
  const [messages, setMessages] = useState([])
  const { user } = useAuth()
  const connection = useRef(null)
  const messagesContainerRef = useRef(null)

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await GetChatByMe()
        if (response.statusCode === 200) {
          setChats(response.data)
        } else {
          console.log('Failed to fetch users:', response.message)
        }
      } catch (error) {
        console.log('Error fetching users:', error)
      }
    }

    fetchUsers()
  }, [messages])

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

  const handleUserClick = async chatFocus => {
    setSelectedChat(chatFocus)
    setMessages(chatFocus.messages) // Set messages from the selected chat
  }

  const handleSendMessage = async () => {
    if (message.trim() === '' || !selectedChat) return

    const apiMessage = {
      ChatId: selectedChat.id,
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
    <Grid container spacing={1} style={{ marginTop: '5px' }}>
      <Grid item xs={3}>
        <Paper
          elevation={3}
          style={{
            height: '89vh',
            overflowY: 'auto',
            display: 'flex',
            flexDirection: 'column'
          }}
        >
          <MessageList
            me={user}
            chats={chats}
            searchText={searchText}
            setSearchText={setSearchText}
            handleUserClick={handleUserClick}
            selectedChat={selectedChat}
          />
        </Paper>
      </Grid>
      <Grid item xs={8.9}>
        <Paper
          elevation={3}
          style={{
            height: '89vh',
            display: 'flex',
            flexDirection: 'column'
          }}
        >
          {selectedChat && (
            <MessageInfo selectedChat={selectedChat} me={user} />
          )}
          <ChatBox
            me={user}
            selectedChat={selectedChat}
            messages={messages}
            messagesContainerRef={messagesContainerRef}
            getMessageProps={getMessageProps}
          />
          <MessageInput
            message={message}
            setMessage={setMessage}
            handleSendMessage={handleSendMessage}
          />
        </Paper>
      </Grid>
    </Grid>
  )
}

export default Chat
