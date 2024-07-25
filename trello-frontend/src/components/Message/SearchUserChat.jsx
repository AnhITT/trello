import TextField from '@mui/material/TextField'
import InputAdornment from '@mui/material/InputAdornment'
import SearchIcon from '@mui/icons-material/Search'
import CloseIcon from '@mui/icons-material/Close'

const SearchUserChat = ({ searchText, setSearchText }) => (
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
          sx={{ cursor: 'pointer' }}
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
)

export default SearchUserChat
