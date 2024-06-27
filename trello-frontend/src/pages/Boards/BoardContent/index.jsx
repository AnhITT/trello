import Box from '@mui/material/Box'
import ListWorkflows from '~/components/Boards/ListWorkflows'
import { UpdateWorkflowPosition } from '~/apis/Workflow'
import {
  DndContext,
  MouseSensor,
  TouchSensor,
  useSensor,
  useSensors,
  DragOverlay,
  defaultDropAnimationSideEffects
} from '@dnd-kit/core'
import { arrayMove } from '@dnd-kit/sortable'
import { useState, useEffect } from 'react'
import Workflow from '~/components/Boards/Workflow'
import Card from '~/components/Card/Card'

const ACTIVE_DRAG_ITEM_TYPE = {
  COLUMN: 'ACTIVE_DRAG_ITEM_TYPE_COLUMN',
  CARD: 'ACTIVE_DRAG_ITEM_TYPE_CARD'
}

function BoardContent({ board }) {
  const mouseSensor = useSensor(MouseSensor, {
    activationConstraint: {
      distance: 10
    }
  })
  // Nhấn giữ 250ms (delay) và dung sai (tolerance) của cảm ứng 500px (dễ hiểu là di chuyển/chênh lệch 5px) thì mới kích hoạt event
  const touchSensor = useSensor(TouchSensor, {
    activationConstraint: {
      delay: 250,
      tolerance: 500
    }
  })
  // Ưu tiên sử dụng kết hợp 2 loại sensors là mouse và touch để có trải nghiệm trên mobile tốt nhất, không bị bug
  // const sensors = useSensors(pointerSensor)
  const sensors = useSensors(mouseSensor, touchSensor)
  const [orderedWorkflows, setOrderedWorkflows] = useState([])

  // Cùng một thời điểm chỉ có một phần tử đang được kéo (column hoặc card)
  const [activeDragItemId, setActiveDragItemId] = useState([null])
  const [activeDragItemType, setActiveDragItemType] = useState([null])
  const [activeDragItemData, setActiveDragItemData] = useState([null])
  const [oldColumnWhenDraggingCard, setOldColumnWhenDraggingCard] = useState([
    null
  ])

  useEffect(() => {
    if (board) {
      setOrderedWorkflows(board.workflows)
    }
  }, [board])
  const findColumnByCardId = cardId => {
    // Đoạn này cần lưu ý, nên dùng c.cards thay vì c.cardOrderIds bởi vì ở bước handleDragOver chúng ta sẽ làm dữ liệu cho cards hoàn chỉnh trước rồi mới tạo ra cardOrderIds mới.
    return orderedWorkflows.find(workflow =>
      workflow?.cards?.map(card => card.id)?.includes(cardId)
    )
  }
  // Trigger khi bắt đầu kéo (drap) một phần tử
  const handleDragStart = event => {
    setActiveDragItemId(event?.active?.id)
    setActiveDragItemType(
      event?.active?.data?.current?.columnId
        ? ACTIVE_DRAG_ITEM_TYPE.CARD
        : ACTIVE_DRAG_ITEM_TYPE.COLUMN
    )
    setActiveDragItemData(event?.active?.data?.current)
    // Nếu là kéo card thì mới thực hiện hành động set giá trị oldColumn
    if (event?.active?.data?.current?.columnId) {
      setOldColumnWhenDraggingCard(findColumnByCardId(event?.active?.id))
    }
  }

  const handleDragOver = event => {
    // Không làm gì nếu kéo workflow
    if (activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN) return
    const { active, over } = event
  }

  const handleDragEnd = event => {
    const { active, over } = event
    if (!active || !over) return
    if (active.id !== over.id) {
      const oldIndex = orderedWorkflows.findIndex(
        workflow => workflow.id === active.id
      )
      const newIndex = orderedWorkflows.findIndex(
        workflow => workflow.id === over.id
      )
      const dndOrderedWorkflows = arrayMove(
        orderedWorkflows,
        oldIndex,
        newIndex
      )
      setOrderedWorkflows(dndOrderedWorkflows)
      try {
        const request = {
          WorkflowId: active.id,
          BoardId: board.id,
          NewPosition: newIndex
        }
        UpdateWorkflowPosition(request)
      } catch (error) {
        setOrderedWorkflows(orderedWorkflows)
      }
    }
    setActiveDragItemId(null)
    setActiveDragItemType(null)
    setActiveDragItemData(null)
  }
  const customDropAnimation = {
    sideEffects: defaultDropAnimationSideEffects({
      styles: {
        active: {
          opacity: '0.5'
        }
      }
    })
  }
  return (
    <DndContext
      onDragStart={handleDragStart}
      onDragOver={handleDragOver}
      onDragEnd={handleDragEnd}
      sensors={sensors}
    >
      <Box
        sx={{
          bgcolor: theme =>
            theme.palette.mode === 'dark' ? '#34495e' : '#1976d2',
          width: '100%',
          height: theme => theme.trello.boardContentHeight,
          p: '10px 0'
        }}
      >
        <ListWorkflows workflows={orderedWorkflows} />
        <DragOverlay dropAnimation={customDropAnimation}>
          {!activeDragItemType && null}
          {activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN && (
            <Workflow workflow={activeDragItemData} />
          )}
          {activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.CARD && (
            <Card card={activeDragItemData} />
          )}
        </DragOverlay>
      </Box>
    </DndContext>
  )
}

export default BoardContent
