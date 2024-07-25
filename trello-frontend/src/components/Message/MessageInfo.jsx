import Avatar from '@mui/material/Avatar'
import CallIcon from '@mui/icons-material/Call'
import VideoCallIcon from '@mui/icons-material/VideoCall'
import InfoIcon from '@mui/icons-material/Info'

const MessageInfo = ({ selectedChat, me }) => {
  if (!selectedChat) return null

  const isGroupChat = selectedChat.isGroup
  let displayName = ''
  let avatarUrl =
    'https://cdn.iconscout.com/icon/free/png-256/free-avatar-370-456322.png'

  if (isGroupChat) {
    // Lấy danh sách tên các thành viên, ngoại trừ người dùng hiện tại
    const memberNames = selectedChat.members
      .filter(member => member.id !== me.Id)
      .map(member => member.lastName || 'Unknown')
      .join(', ')

    displayName = selectedChat.nameGroup || memberNames
    avatarUrl =
      selectedChat.avatarGroup ||
      'https://www.shareicon.net/data/512x512/2016/06/30/788858_group_512x512.png'
  } else {
    const otherMember = selectedChat.members.find(member => member.id !== me.Id)
    displayName = otherMember?.lastName || 'Unknown'
    avatarUrl = otherMember?.avatarUrl || avatarUrl
  }

  return (
    <div
      style={{
        padding: '10px',
        borderBottom: '1px solid rgb(196, 196, 196)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between'
      }}
    >
      <div style={{ display: 'flex', alignItems: 'center' }}>
        <Avatar
          sx={{ width: 40, height: 40, marginRight: '10px' }}
          src={avatarUrl}
        />
        <div>
          <h4 style={{ margin: 0 }}>{displayName}</h4>
          <p style={{ margin: 0, fontSize: '14px', color: '#666' }}>Online</p>
        </div>
      </div>
      <div style={{ display: 'flex', alignItems: 'center' }}>
        <CallIcon sx={{ marginRight: '10px', cursor: 'pointer' }} />
        <VideoCallIcon
          sx={{ marginRight: '10px', cursor: 'pointer', fontSize: '30px' }}
        />
        <InfoIcon sx={{ marginRight: '10px', cursor: 'pointer' }} />
      </div>
    </div>
  )
}

export default MessageInfo
