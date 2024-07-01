import Chip from '@mui/material/Chip'
import Box from '@mui/material/Box'
import DashboardIcon from '@mui/icons-material/Dashboard'
import VpnLockIcon from '@mui/icons-material/VpnLock'
import AddToDriveIcon from '@mui/icons-material/AddToDrive'
import BoltIcon from '@mui/icons-material/Bolt'
import FilterListIcon from '@mui/icons-material/FilterList'
import Avatar from '@mui/material/Avatar'
import AvatarGroup from '@mui/material/AvatarGroup'
import { Button, Tooltip } from '@mui/material'
import PersonAddIcon from '@mui/icons-material/PersonAdd'
import { capitalizeFirstLetter } from '~/utils/formatters'

const MENU_STYLES = {
  color: 'white',
  bgcolor: 'transparent',
  border: 'none',
  paddingX: '5px',
  borderRadius: '4px',
  '.MuiSvgIcon-root': {
    color: 'white'
  },
  '&:hover': {
    color: 'primary.50'
  }
}
function BoardBar({ board }) {
  return (
    <Box
      px={2}
      sx={{
        width: '100%',
        height: theme => theme.trello.boardBarHeight,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        gap: 2,
        overflowX: 'auto',
        bgcolor: theme =>
          theme.palette.mode === 'dark' ? '#34495e' : '#1976d2'
      }}
    >
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          gap: 2
        }}
      >
        <Chip
          icon={<DashboardIcon />}
          label={board?.name}
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
        <Chip
          icon={<VpnLockIcon />}
          label={capitalizeFirstLetter(board?.id)}
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
        <Chip
          icon={<AddToDriveIcon />}
          label="Add To Google Drive"
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
        <Chip
          icon={<BoltIcon />}
          label="Automation"
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
        <Chip
          icon={<FilterListIcon />}
          label="Filters"
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
      </Box>
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          gap: 2
        }}
      >
        <Button
          sx={{
            color: 'white',
            borderColor: 'white',
            '&:hover': { borderColor: 'white' }
          }}
          variant="outlined"
          startIcon={<PersonAddIcon />}
        >
          Invite
        </Button>
        <AvatarGroup
          max={7}
          sx={{
            '& .MuiAvatar-root': {
              width: 34,
              height: 34,
              fontSize: 16,
              border: 'none',
              color: 'white',
              cursor: 'pointer',
              '&:first-of-type': { bgcolor: '#a4b0be' }
            }
          }}
        >
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Remy Sharp"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Travis Howard"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=moZi9S5YqiAQ7kNvgHwGepX&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYDK3MxPXHOTO4eDSqTgpvydd-VofOR-SIknXa-YBO-fAg&oe=6688093D"
            />
          </Tooltip>
        </AvatarGroup>
      </Box>
    </Box>
  )
}

export default BoardBar
