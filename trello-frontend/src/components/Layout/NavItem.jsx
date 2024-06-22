import { NavLink as RouterLink } from 'react-router-dom'
import React from 'react'
import Button from '@mui/material/Button'
import Menu from '@mui/material/Menu'
import MenuItem from '@mui/material/MenuItem'
import Paper from '@mui/material/Paper'
import Divider from '@mui/material/Divider'
import MenuList from '@mui/material/MenuList'
import ListItemIcon from '@mui/material/ListItemIcon'
import ListItemText from '@mui/material/ListItemText'
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown'
import Check from '@mui/icons-material/Check'

function NavItem({ item, index }) {
  const [anchorEl, setAnchorEl] = React.useState(null)

  const handleClick = event => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  return (
    <React.Fragment key={index}>
      {item.children ? (
        <>
          <Button
            aria-controls={`menu-${index}`}
            aria-haspopup="true"
            onClick={handleClick}
            sx={{
              fontSize: '0.8rem',
              fontWeight: 'bold',
              color: 'primary.A700'
            }}
            endIcon={<KeyboardArrowDownIcon />}
          >
            {item.title}
          </Button>
          <Menu
            id={`menu-${index}`}
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleClose}
            MenuListProps={{
              'aria-labelledby': `menu-${index}`
            }}
          >
            <Paper sx={{ width: 320 }}>
              <MenuList dense>
                {item.children.map((child, idx) => (
                  <MenuItem
                    key={idx}
                    component={RouterLink}
                    to={child.href}
                    onClick={handleClose}
                  >
                    <ListItemText inset>{child.title}</ListItemText>
                  </MenuItem>
                ))}
                <MenuItem>
                  <ListItemText inset>1.15</ListItemText>
                </MenuItem>
                <MenuItem>
                  <ListItemText inset>Double</ListItemText>
                </MenuItem>
                <MenuItem>
                  <ListItemIcon>
                    <Check />
                  </ListItemIcon>
                  Custom: 1.2
                </MenuItem>
                <Divider />
                <MenuItem>
                  <ListItemText>Add space before paragraph</ListItemText>
                </MenuItem>
                <MenuItem>
                  <ListItemText>Add space after paragraph</ListItemText>
                </MenuItem>
                <Divider />
                <MenuItem>
                  <ListItemText>Custom spacing...</ListItemText>
                </MenuItem>
              </MenuList>
            </Paper>
          </Menu>
        </>
      ) : (
        <Button
          component={RouterLink}
          to={item.href}
          sx={{
            fontSize: '0.8rem',
            fontWeight: 'bold',
            color: 'primary.A700'
          }}
        >
          {item.title}
        </Button>
      )}
    </React.Fragment>
  )
}

export default NavItem
