import { experimental_extendTheme as extendTheme } from '@mui/material/styles'
import { teal, deepOrange, cyan, orange } from '@mui/material/colors'
const theme = extendTheme({
  trello: {
    appBarHeight: '55px',
    boardBarHeight: '65px'
  },
  colorSchemes: {
    light: {
      palette: {
        primary: teal,
        secondary: deepOrange
      }
    },
    dark: {
      palette: {
        palette: {
          primary: cyan,
          secondary: orange
        }
      }
    }
  }
})
export default theme
