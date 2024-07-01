import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Workflows from './Workflow'
import CloseIcon from '@mui/icons-material/Close'
import NoteAddIcon from '@mui/icons-material/NoteAdd'
import { TextField } from '@mui/material'
import { toast } from 'react-toastify'
import { useState } from 'react'
import {
  SortableContext,
  horizontalListSortingStrategy
} from '@dnd-kit/sortable'

function ListWorkflows({
  workflows,
  createNewWorkflow,
  createNewTaskCard,
  deleteWorkflow
}) {
  const [openNewWorkflowForm, setOpenNewWorkflowForm] = useState(false)
  const toggleOpenNewWorkflowForm = () =>
    setOpenNewWorkflowForm(!openNewWorkflowForm)

  const [newWorkflowTitle, setNewWorkflowTitle] = useState('')

  const addNewWorkflow = () => {
    if (!newWorkflowTitle) {
      toast('🦄 Please enter Workflow Title!', {
        position: 'bottom-right',
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        theme: 'light'
      })
      return
    }
    const newWorkflow = {
      name: newWorkflowTitle
    }
    createNewWorkflow(newWorkflow)
    toggleOpenNewWorkflowForm()
    setNewWorkflowTitle('')
  }
  return (
    <SortableContext
      items={workflows?.map(c => c.id)}
      strategy={horizontalListSortingStrategy}
    >
      <Box
        sx={{
          bgcolor: 'inherit',
          width: '100%',
          height: '100%',
          display: 'flex',
          overflowX: 'auto',
          overflowY: 'hidden',
          '&::-webkit-scrollbar-track': { m: 2 }
        }}
      >
        {workflows?.map(workflow => (
          <Workflows
            key={workflow.id}
            workflow={workflow}
            createNewTaskCard={createNewTaskCard}
            deleteWorkflow={deleteWorkflow}
          />
        ))}

        {/* Box Add new Workflow CTA */}
        {!openNewWorkflowForm ? (
          <Box
            onClick={toggleOpenNewWorkflowForm}
            sx={{
              minWidth: '250px',
              maxWidth: '250px',
              mx: 2,
              borderRadius: '6px',
              height: 'fit-content',
              bgcolor: '#ffffff3d'
            }}
          >
            <Button
              startIcon={<NoteAddIcon />}
              sx={{
                color: 'white',
                width: '100%',
                justifyContent: 'flex-start',
                pl: 2.5,
                py: 1
              }}
            >
              Add new Workflow
            </Button>
          </Box>
        ) : (
          <Box
            sx={{
              minWidth: '250px',
              maxWidth: '250px',
              mx: 2,
              p: 1,
              borderRadius: '6px',
              height: 'fit-content',
              bgcolor: '#ffffff3d',
              display: 'flex',
              flexDirection: 'column',
              gap: 1
            }}
          >
            <TextField
              label="Enter workflow title..."
              type="text"
              size="small"
              variant="outlined"
              autoFocus
              value={newWorkflowTitle}
              onChange={e => setNewWorkflowTitle(e.target.value)}
              sx={{
                '& label': { color: 'white' },
                '& input': { color: 'white' },
                '& label.Mui-focused': { color: 'white' },
                '& .MuiOutlinedInput-root': {
                  '& fieldset': { borderColor: 'white' },
                  '&:hover fieldset': { borderColor: 'white' },
                  '&.Mui-focused fieldset': { borderColor: 'white' }
                }
              }}
            />
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 1
              }}
            >
              <Button
                onClick={addNewWorkflow}
                variant="contained"
                color="success"
                size="small"
                sx={{
                  boxShadow: 'none',
                  border: '0.5px solid',
                  borderColor: theme => theme.palette.success.main,
                  '&:hover': { bgcolor: theme => theme.palette.success.main }
                }}
              >
                Add Workflow
              </Button>
              <CloseIcon
                fontSize="small"
                sx={{
                  color: 'white',
                  cursor: 'pointer',
                  '&:hover': { color: theme => theme.palette.warning.light }
                }}
                onClick={toggleOpenNewWorkflowForm}
              />
            </Box>
          </Box>
        )}
      </Box>
    </SortableContext>
  )
}

export default ListWorkflows
