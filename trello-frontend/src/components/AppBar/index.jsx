import { useState } from 'react'
import Box from '@mui/material/Box'
import ModeSelect from '../ModeSelect'
import AppsIcon from '@mui/icons-material/Apps'
import TrelloIcon from '~/assets/svg/trello.svg?react'
import SvgIcon from '@mui/material/SvgIcon'
import Typography from '@mui/material/Typography'
import Button from '@mui/material/Button'
import Workspaces from './Menus/Workspaces'
import Starred from './Menus/Starred'
import Templates from './Menus/Templates'
import Reccent from './Menus/Reccent'
import AccountMenu from './Menus/AccountMenu'
import { Badge, InputAdornment, TextField, Tooltip } from '@mui/material'
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone'
import HelpOutlineIcon from '@mui/icons-material/HelpOutline'
import SearchIcon from '@mui/icons-material/Search'
import LibraryAddIcon from '@mui/icons-material/LibraryAdd'
import CloseIcon from '@mui/icons-material/Close'

function AppBar() {
  const [searchText, setSearchText] = useState('')
  return (
    <Box
      px={2}
      sx={{
        width: '100%',
        height: theme => theme.trello.appBarHeight,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        overflowX: 'auto',
        bgcolor: theme =>
          theme.palette.mode === 'dark' ? '#222f3e' : ' hsl(215,90%,32.7%)'
      }}
    >
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          gap: 2
        }}
      >
        <AppsIcon sx={{ color: 'white' }} />
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            gap: 0.5
          }}
        >
          <SvgIcon
            component={TrelloIcon}
            fontSize="small"
            sx={{ color: 'white' }}
          />
          <Typography
            variant="span"
            sx={{
              fontSize: '1.2rem',
              fontWeight: 'bold',
              color: 'white'
            }}
          >
            Trello
          </Typography>
          <Workspaces />
          <Reccent />
          <Starred />
          <Templates />
          <Button
            sx={{
              color: 'white',
              fontWeight: 'bold',
              border: 'none',
              '&:hover': { border: 'none' }
            }}
            variant="outlined"
            startIcon={<LibraryAddIcon />}
          >
            Create
          </Button>
        </Box>
      </Box>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
        <TextField
          id="outlined-search"
          label="Search"
          type="text"
          value={searchText}
          size="small"
          onChange={e => setSearchText(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ color: 'white' }} />
              </InputAdornment>
            ),
            endAdornment: (
              <CloseIcon
                fontSize="small"
                sx={{
                  color: searchText ? 'white' : 'transparent',
                  cursor: 'pointer'
                }}
                onClick={() => setSearchText('')}
              />
            )
          }}
          sx={{
            minWidth: '120px',
            maxWidth: '180px',
            '& label': { color: 'white' },
            '& input': { color: 'white' },
            '& label.Mui-focused': { color: 'white' },
            '& .MuiOutlinedInput-root': {
              '& fieldset': {
                borderColor: 'white'
              },
              '&:hover fieldset': {
                borderColor: 'white'
              },
              '&.Mui-focused fieldset': {
                borderColor: 'white'
              }
            }
          }}
        />
        <ModeSelect />
        <Tooltip title="Notification">
          <Badge
            color="warning"
            variant="dot"
            sx={{ cursor: 'pointer', color: 'white' }}
          >
            <NotificationsNoneIcon />
          </Badge>
        </Tooltip>
        <Tooltip title="Help">
          <Badge color="secondary" sx={{ cursor: 'pointer', color: 'white' }}>
            <HelpOutlineIcon />
          </Badge>
        </Tooltip>
        <AccountMenu />
      </Box>
    </Box>
  )
}

export default AppBar
