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
          theme.palette.mode === 'dark' ? '#34495e' : 'hsla(215,90%,37.7%,0.9)'
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
          label={board?.title}
          onClick={() => {}}
          sx={MENU_STYLES}
        ></Chip>
        <Chip
          icon={<VpnLockIcon />}
          label={capitalizeFirstLetter(board?.type)}
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
          icon={<BoltIcon />}
          label="Automation"
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
              src="https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/428624280_1080284179970282_4783097439728646945_n.jpg?_nc_cat=110&ccb=1-7&_nc_sid=5f2048&_nc_ohc=sVNFF65bLcsQ7kNvgG1ZfiN&_nc_ht=scontent.fsgn2-6.fna&oh=00_AYBRKfTOk6O2TSb9daoHfDTlRbVZAHDWR20O2dCuUH93cw&oe=667B4ABD"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Travis Howard"
              src="https://scontent.fsgn2-7.fna.fbcdn.net/v/t39.30808-6/448890771_976158761181026_3217766263655999592_n.jpg?_nc_cat=108&ccb=1-7&_nc_sid=5f2048&_nc_ohc=_yC7x78hKYMQ7kNvgGHG6Of&_nc_ht=scontent.fsgn2-7.fna&oh=00_AYDwCgAQqB9Hs1PZvzQwGJU0lpHCS2QM8ztk90tukjHIWA&oe=667B7839"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-4.fna.fbcdn.net/v/t39.30808-6/448795593_893119859494647_7227545967017129160_n.jpg?_nc_cat=101&ccb=1-7&_nc_sid=5f2048&_nc_ohc=XvgYqqDDGbgQ7kNvgGTONTx&_nc_ht=scontent.fsgn2-4.fna&oh=00_AYBRlpA28gW_jbmPUnykVV4OmH-TsjHrm1tYQmeMmIUQEg&oe=667B537E"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-4.fna.fbcdn.net/v/t39.30808-6/448795593_893119859494647_7227545967017129160_n.jpg?_nc_cat=101&ccb=1-7&_nc_sid=5f2048&_nc_ohc=XvgYqqDDGbgQ7kNvgGTONTx&_nc_ht=scontent.fsgn2-4.fna&oh=00_AYBRlpA28gW_jbmPUnykVV4OmH-TsjHrm1tYQmeMmIUQEg&oe=667B537E"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-4.fna.fbcdn.net/v/t39.30808-6/448795593_893119859494647_7227545967017129160_n.jpg?_nc_cat=101&ccb=1-7&_nc_sid=5f2048&_nc_ohc=XvgYqqDDGbgQ7kNvgGTONTx&_nc_ht=scontent.fsgn2-4.fna&oh=00_AYBRlpA28gW_jbmPUnykVV4OmH-TsjHrm1tYQmeMmIUQEg&oe=667B537E"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Cindy Baker"
              src="https://scontent.fsgn2-4.fna.fbcdn.net/v/t39.30808-6/448795593_893119859494647_7227545967017129160_n.jpg?_nc_cat=101&ccb=1-7&_nc_sid=5f2048&_nc_ohc=XvgYqqDDGbgQ7kNvgGTONTx&_nc_ht=scontent.fsgn2-4.fna&oh=00_AYBRlpA28gW_jbmPUnykVV4OmH-TsjHrm1tYQmeMmIUQEg&oe=667B537E"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-9.fna.fbcdn.net/v/t39.30808-6/448763436_475648505011295_2737609775240397127_n.jpg?stp=cp6_dst-jpg&_nc_cat=106&ccb=1-7&_nc_sid=5f2048&_nc_ohc=32pcK9t5xiQQ7kNvgEvUQUT&_nc_ht=scontent.fsgn2-9.fna&oh=00_AYDSP4-CzXPpDClbHawVpVFErOMmGU5T7JcZ9z_0XIVQwQ&oe=667B5E5C"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-9.fna.fbcdn.net/v/t39.30808-6/448763436_475648505011295_2737609775240397127_n.jpg?stp=cp6_dst-jpg&_nc_cat=106&ccb=1-7&_nc_sid=5f2048&_nc_ohc=32pcK9t5xiQQ7kNvgEvUQUT&_nc_ht=scontent.fsgn2-9.fna&oh=00_AYDSP4-CzXPpDClbHawVpVFErOMmGU5T7JcZ9z_0XIVQwQ&oe=667B5E5C"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-9.fna.fbcdn.net/v/t39.30808-6/448763436_475648505011295_2737609775240397127_n.jpg?stp=cp6_dst-jpg&_nc_cat=106&ccb=1-7&_nc_sid=5f2048&_nc_ohc=32pcK9t5xiQQ7kNvgEvUQUT&_nc_ht=scontent.fsgn2-9.fna&oh=00_AYDSP4-CzXPpDClbHawVpVFErOMmGU5T7JcZ9z_0XIVQwQ&oe=667B5E5C"
            />
          </Tooltip>
          <Tooltip title="Lam Anh">
            <Avatar
              alt="Agnes Walker"
              src="https://scontent.fsgn2-9.fna.fbcdn.net/v/t39.30808-6/448763436_475648505011295_2737609775240397127_n.jpg?stp=cp6_dst-jpg&_nc_cat=106&ccb=1-7&_nc_sid=5f2048&_nc_ohc=32pcK9t5xiQQ7kNvgEvUQUT&_nc_ht=scontent.fsgn2-9.fna&oh=00_AYDSP4-CzXPpDClbHawVpVFErOMmGU5T7JcZ9z_0XIVQwQ&oe=667B5E5C"
            />
          </Tooltip>
        </AvatarGroup>
      </Box>
    </Box>
  )
}

export default BoardBar
