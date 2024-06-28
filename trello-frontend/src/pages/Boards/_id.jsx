import Container from '@mui/material/Container'
import AppBar from '~/components/AppBar'
import BoardBar from '~/pages/Boards/BoardBar'
import BoardContent from '~/pages/Boards/BoardContent'
import { GetAllProptiesFromBoard } from '~/apis/Board'
import { useState, useEffect } from 'react'
import { isEmpty } from 'lodash'
import { generatePlaceholderCard } from '~/utils/formatters'
import { AddWorkflow } from '~/apis/Workflow'
import { UpdateWorkflowPosition } from '~/apis/Workflow'
import { UpdateTaskCardPosition } from '~/apis/TaskCard'

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

  const moveWorkflows = data => {
    UpdateWorkflowPosition(data)
  }
  return (
    <Container disableGutters maxWidth="false" sx={{ height: '100vh' }}>
      <AppBar />
      <BoardBar board={board} />
      <BoardContent
        board={board}
        createNewWorkflow={createNewWorkflow}
        moveWorkflows={moveWorkflows}
      />
    </Container>
  )
}

export default Board
