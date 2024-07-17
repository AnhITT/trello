import AppBar from '@mui/material/AppBar'
import { Box, Typography } from '@mui/material'
import TrelloIcon from '~/assets/svg/trello.svg?react'
import { NavLink as RouterLink } from 'react-router-dom'
import { Outlet } from 'react-router-dom'
import SvgIcon from '@mui/material/SvgIcon'

const MainLayout = () => {
  return (
    <div>
      <AppBar position="static">
        <Box
          component={RouterLink}
          to="/board"
          sx={{
            display: 'flex',
            justifyContent: 'start',
            width: '100%',
            height: '40px',
            m: 2,
            cursor: 'pointer',
            textDecoration: 'none'
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <SvgIcon
              component={TrelloIcon}
              sx={{ width: 40, height: 40, mr: 1, color: 'white' }}
            />
            <Typography
              variant="h3"
              component="div"
              sx={{
                fontSize: '2.5rem',
                fontWeight: 'bold',
                color: 'white'
              }}
            >
              Trello
            </Typography>
          </Box>
        </Box>
      </AppBar>
      <Outlet />
    </div>
  )
}

export default MainLayout
