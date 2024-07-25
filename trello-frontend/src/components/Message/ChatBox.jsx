import ListItem from '@mui/material/ListItem'
import Message from '~/components/Message/Message'

const ChatBox = ({
  me,
  selectedChat,
  messages,
  messagesContainerRef,
  getMessageProps
}) => (
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
          const senderInfo = selectedChat.members.find(
            member => member.id === msg.sender
          )
          return (
            <Message
              key={index}
              me={me.Id}
              sender={msg.sender}
              senderName={senderInfo ? senderInfo.lastName : 'Unknown'}
              senderAvatar={senderInfo ? senderInfo.avatar : null}
              text={msg.text}
              time={msg.createdDate}
              isFirstInGroup={isFirstInGroup}
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
          style={{
            padding: '10px',
            borderRadius: '10px',
            border: '1px solid #ddd',
            backgroundColor: '#f5f5f5',
            cursor: 'pointer'
          }}
        >
          Start a conversation with{' '}
          {selectedChat?.members
            .filter(member => member.id !== me.Id)
            .map(member => member.lastName)
            .join(', ')}
        </button>
      </ListItem>
    )}
  </div>
)

export default ChatBox
