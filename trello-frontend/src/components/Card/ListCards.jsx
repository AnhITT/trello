import Box from '@mui/material/Box'
import Card from './Card'
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable'

function ListCards({ cards }) {
  return (
    <SortableContext
      items={cards?.map(w => w.id)}
      strategy={verticalListSortingStrategy}
    >
      <Box
        sx={{
          p: '0 5px', // padding
          m: '0 5px', // margin
          display: 'flex',
          flexDirection: 'column',
          gap: 1,
          overflowX: 'hidden',
          overflowY: 'auto',
          maxHeight: theme => `calc(
       ${theme.trello.boardContentHeight} -
       ${theme.spacing(5)} -
       ${theme.trello.columnHeaderHeight} -
       ${theme.trello.columnFooterHeight}
     )`,
          '&::-webkit-scrollbar-thumb': { backgroundColor: '#ced0da' },
          '&::-webkit-scrollbar-thumb:hover': { backgroundColor: '#dfc2cf' }
        }}
      >
        {cards?.map(item => (
          <Card key={item.id} card={item} />
        ))}
      </Box>
    </SortableContext>
  )
}

export default ListCards
