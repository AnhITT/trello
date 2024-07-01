import CommentIcon from '@mui/icons-material/Comment'
import AttachmentIcon from '@mui/icons-material/Attachment'
import GroupIcon from '@mui/icons-material/Group'
import CardActions from '@mui/material/CardActions'
import CardMedia from '@mui/material/CardMedia'
import { Card as MuiCard } from '@mui/material'
import CardContent from '@mui/material/CardContent'
import Typography from '@mui/material/Typography'
import Button from '@mui/material/Button'
import { useSortable } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import CheckBoxOutlineBlankIcon from '@mui/icons-material/CheckBoxOutlineBlank'
import CheckBoxIcon from '@mui/icons-material/CheckBox'
function Card({ card }) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging
  } = useSortable({ id: card.id, data: { ...card } })

  const shouldShowCardAction = () => {
    return (
      card?.userCount != 0 || card?.totalChecklistItems != 0 || card?.files != 0
    )
  }

  const dndKitCardStyles = {
    transform: CSS.Translate.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : undefined,
    border: isDragging ? '1px solid #2ecc71' : undefined
  }

  return (
    <MuiCard
      ref={setNodeRef}
      style={dndKitCardStyles}
      {...attributes}
      {...listeners}
      sx={{
        cursor: 'pointer',
        boxShadow: '0 1px 1px rgba(0,0,0,0.2)',
        overflow: 'unset',
        display: card?.FE_PlaceholderCard ? 'none' : 'block',
        border: '1px solid transparent',
        '&:hover': { borderColor: theme => theme.palette.primary.main }
      }}
    >
      {card?.cover && <CardMedia sx={{ height: 140 }} image={card?.cover} />}

      <CardContent sx={{ p: 1.5, '&:last-child': { p: 1.5 } }}>
        <Typography> {card?.title}</Typography>
      </CardContent>
      {shouldShowCardAction() && (
        <CardActions sx={{ p: '0 4px 8px 4px' }}>
          {card?.userCount != 0 && (
            <Button size="small" startIcon={<GroupIcon />}>
              {card?.userCount}
            </Button>
          )}

          {card?.totalChecklistItems !== 0 &&
            (card?.totalChecklistItems === card?.completedChecklistItems ? (
              // Điều kiện khi totalChecklistItems === completedChecklistItems
              <Button size="small" startIcon={<CheckBoxIcon />}>
                {card?.completedChecklistItems}/{card?.totalChecklistItems}
              </Button>
            ) : (
              // Điều kiện khi totalChecklistItems !== completedChecklistItems
              <Button size="small" startIcon={<CheckBoxOutlineBlankIcon />}>
                {card?.completedChecklistItems}/{card?.totalChecklistItems}
              </Button>
            ))}

          {card?.files !== 0 && (
            <Button size="small" startIcon={<AttachmentIcon />}>
              {card?.files}
            </Button>
          )}
        </CardActions>
      )}
    </MuiCard>
  )
}

export default Card
