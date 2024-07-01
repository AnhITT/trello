import Container from '@mui/material/Container'
import AppBar from '~/components/AppBar'
import BoardBar from '~/pages/Boards/BoardBar'
import BoardContent from '~/pages/Boards/BoardContent'
import { GetAllProptiesFromBoard } from '~/apis/Board'
import { useState, useEffect } from 'react'
import { toast } from 'react-toastify'
import { isEmpty } from 'lodash'
import { generatePlaceholderCard } from '~/utils/formatters'
import {
  UpdateWorkflowPosition,
  AddWorkflow,
  DeleteWorkflow
} from '~/apis/Workflow'
import { UpdateTaskCardPosition, AddTaskCard } from '~/apis/TaskCard'

function Board() {
  const [board, setBoard] = useState(null)
  useEffect(() => {
    fetchData()
  }, [])

  const fetchData = async () => {
    const boardID = '69e0f424-2245-489c-b77d-53ae358762c4'
    const data = await GetAllProptiesFromBoard(boardID)
    data.data.workflows.forEach(workflow => {
      if (isEmpty(workflow.cards)) {
        workflow.cards = [generatePlaceholderCard(workflow)]
        workflow.cardOrderIds = [generatePlaceholderCard(workflow).id]
      }
    })

    setBoard(data.data)
  }

  const createNewWorkflow = async newWorkflowData => {
    const createdWorkflow = await AddWorkflow({
      ...newWorkflowData,
      boardId: board.id
    })

    createdWorkflow.data.cards = [generatePlaceholderCard(createdWorkflow)]
    createdWorkflow.data.cardOrderIds = [
      generatePlaceholderCard(createdWorkflow).id
    ]
    const newBoard = { ...board }
    newBoard.workflows.push(createdWorkflow.data)
    setBoard(newBoard)
  }

  const createNewTaskCard = async newTaskCardData => {
    const createdCard = await AddTaskCard({
      ...newTaskCardData
    })

    const newBoard = { ...board }
    const workflowIndex = newBoard.workflows.findIndex(
      workflow => workflow.id === createdCard.data.workflowId
    )
    if (workflowIndex !== -1) {
      newBoard.workflows[workflowIndex].cards.push(createdCard.data)
    }
    setBoard(newBoard)
  }

  const moveWorkflows = data => {
    UpdateWorkflowPosition(data)
  }

  const moveCards = data => {
    UpdateTaskCardPosition(data)
  }

  const deleteWorkflow = idWorkflow => {
    const newBoard = { ...board }
    newBoard.workflows = newBoard.workflows.filter(c => c.id !== idWorkflow)
    DeleteWorkflow(idWorkflow)

    setBoard(newBoard)
  }
  return (
    <Container disableGutters maxWidth="false" sx={{ height: '100vh' }}>
      <AppBar />
      <BoardBar board={board} />
      <BoardContent
        board={board}
        createNewWorkflow={createNewWorkflow}
        createNewTaskCard={createNewTaskCard}
        moveWorkflows={moveWorkflows}
        moveCards={moveCards}
        deleteWorkflow={deleteWorkflow}
      />
    </Container>
  )
}

export default Board
