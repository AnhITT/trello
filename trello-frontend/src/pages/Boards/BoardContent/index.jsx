import Box from '@mui/material/Box'
import ListWorkflows from '~/components/Workflows/ListWorkflows'
import { UpdateWorkflowPosition } from '~/apis/Workflow'
import { UpdateTaskCardPosition } from '~/apis/TaskCard'
import {
  DndContext,
  MouseSensor,
  TouchSensor,
  useSensor,
  useSensors,
  DragOverlay,
  defaultDropAnimationSideEffects,
  closestCorners,
  pointerWithin,
  getFirstCollision
} from '@dnd-kit/core'
import { arrayMove } from '@dnd-kit/sortable'
import { useCallback, useEffect, useRef, useState } from 'react'
import Workflow from '~/components/Workflows/Workflow'
import Card from '~/components/Card/Card'
import { cloneDeep, isEmpty } from 'lodash'
import { generatePlaceholderCard } from '~/utils/formatters'

const ACTIVE_DRAG_ITEM_TYPE = {
  WORKFLOW: 'ACTIVE_DRAG_ITEM_TYPE_WORKFLOW',
  CARD: 'ACTIVE_DRAG_ITEM_TYPE_CARD'
}

function BoardContent({ board, createNewWorkflow, moveWorkflows }) {
  const mouseSensor = useSensor(MouseSensor, {
    activationConstraint: {
      distance: 10
    }
  })

  const touchSensor = useSensor(TouchSensor, {
    activationConstraint: {
      delay: 250,
      tolerance: 500
    }
  })

  const sensors = useSensors(mouseSensor, touchSensor)
  const [orderedWorkflows, setOrderedWorkflows] = useState([])

  const [activeDragItemId, setActiveDragItemId] = useState([null])
  const [activeDragItemType, setActiveDragItemType] = useState([null])
  const [activeDragItemData, setActiveDragItemData] = useState([null])
  const [oldWorkflowWhenDraggingCard, setOldWorkflowWhenDraggingCard] =
    useState([null])
  const lastOverId = useRef(null)

  useEffect(() => {
    if (board) {
      setOrderedWorkflows(board.workflows)
    }
  }, [board])

  const findWorkflowByCardId = cardId => {
    return orderedWorkflows.find(workflow =>
      workflow?.cards?.map(card => card.id)?.includes(cardId)
    )
  }

  const moveCardBetweenDifferentWorkflows = (
    overWorkflow,
    overCardId,
    active,
    over,
    activeWorkflow,
    activeDraggingCardId,
    activeDraggingCardData
  ) => {
    setOrderedWorkflows(prevWorkflows => {
      // Tìm vị trí (index) của cái overCard trong Workflow đích (nơi activeCard sắp được thả)
      const overCardIndex = overWorkflow?.cards?.findIndex(
        card => card.id === overCardId
      )

      // Logic tính toán "cardIndex mới" (trên hoặc dưới overCard) lấy chuẩn ra từ code của thư viện - nhiều khi muốn từ chối hiểu =))
      let newCardIndex
      const isBelowOverItem =
        active.rect.current.translated &&
        active.rect.current.translated.top > over.rect.top + over.rect.height
      const modifier = isBelowOverItem ? 1 : 0
      newCardIndex =
        overCardIndex >= 0
          ? overCardIndex + modifier
          : overWorkflow?.cards?.length + 1

      // Clone mảng OrderedWorkflowsState cũ ra một cái mới để xử lý data rồi return - cập nhật lại OrderedWorkflowsState mới
      const nextWorkflows = cloneDeep(prevWorkflows)
      const nextActiveWorkflow = nextWorkflows.find(
        workspace => workspace.id === activeWorkflow.id
      )
      const nextOverWorkflow = nextWorkflows.find(
        workspace => workspace.id === overWorkflow.id
      )

      // nextActiveworkspace: workspace cũ
      if (nextActiveWorkflow) {
        // Xoá card ở cái Workflow active (cũng có thể là Workflow cũ, cái lúc mà kéo card ra khỏi nó để sang Workflow khác)
        nextActiveWorkflow.cards = nextActiveWorkflow.cards.filter(
          card => card.id !== activeDraggingCardId
        )

        // Thêm Placeholder Card nếu workspace rỗng: Bị kéo hết Card đi, không còn cái nào nữa. (Video 37.2)
        if (isEmpty(nextActiveWorkflow.cards)) {
          nextActiveWorkflow.cards = [
            generatePlaceholderCard(nextActiveWorkflow)
          ]
        }
        nextActiveWorkflow.cardOrderIds = nextActiveWorkflow.cards.map(
          card => card.id
        )
      }

      // nextOverWorkflow: Workflow mới
      if (nextOverWorkflow) {
        // Kiểm tra xem cái card đang kéo nó có tồn tại ở overWorkflow chưa, nếu có thì cần xoá nó trước
        nextOverWorkflow.cards = nextOverWorkflow.cards.filter(
          card => card.id !== activeDraggingCardId
        )

        // Phải cập nhật lại chuẩn dữ liệu workspaceId trong card sau khi kéo card giữa 2 workspace khác nhau
        const rebuild_activeDraggingCardData = {
          ...activeDraggingCardData,
          workspaceId: nextOverWorkflow.id
        }

        // Tiếp theo là thêm cái card đang kéo vào overWorkflow theo vị trí index mới
        nextOverWorkflow.cards = nextOverWorkflow.cards.toSpliced(
          newCardIndex,
          0,
          rebuild_activeDraggingCardData
        )

        // Xoá cái Placeholder Card đi nếu nó đang tồn tại (Video 37.2)
        nextOverWorkflow.cards = nextOverWorkflow.cards.filter(
          card => !card.FE_PlaceholderCard
        )

        // Cập nhật lại mảng cardOrderIds cho chuẩn dữ liệu
        nextOverWorkflow.cardOrderIds = nextOverWorkflow.cards.map(
          card => card.id
        )
      }

      return nextWorkflows
    })
  }

  const handleDragStart = event => {
    setActiveDragItemId(event?.active?.id)
    setActiveDragItemType(
      event?.active?.data?.current?.workflowId
        ? ACTIVE_DRAG_ITEM_TYPE.CARD
        : ACTIVE_DRAG_ITEM_TYPE.WORKFLOW
    )
    setActiveDragItemData(event?.active?.data?.current)
    // Nếu là kéo card thì mới thực hiện hành động set giá trị oldWorkflow
    if (event?.active?.data?.current?.workflowId) {
      setOldWorkflowWhenDraggingCard(findWorkflowByCardId(event?.active?.id))
    }
  }

  const handleDragOver = event => {
    if (activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.WORKFLOW) return
    const { active, over } = event
    if (!active || !over) return

    const {
      id: activeDraggingCardId,
      data: { current: activeDraggingCardData }
    } = active

    const { id: overCardId } = over

    const activeWorkflow = findWorkflowByCardId(activeDraggingCardId)
    const overWorkflow = findWorkflowByCardId(overCardId)
    if (!activeWorkflow || !overWorkflow) return

    if (activeWorkflow.id !== overWorkflow.id) {
      moveCardBetweenDifferentWorkflows(
        overWorkflow,
        overCardId,
        active,
        over,
        activeWorkflow,
        activeDraggingCardId,
        activeDraggingCardData
      )
    }
  }

  const handleDragEnd = event => {
    const { active, over } = event

    if (!active || !over) return

    if (activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.CARD) {
      const {
        id: activeDraggingCardId,
        data: { current: activeDraggingCardData }
      } = active

      // overCard: là cái card đang tương tác trên hoặc dưới so với cái card được kéo ở trên
      const { id: overCardId } = over

      // Tìm 2 cái column theo cái cardId
      const activeWorkflow = findWorkflowByCardId(activeDraggingCardId)
      const overWorkflow = findWorkflowByCardId(overCardId)
      // Nếu không tồn tại 1 trong 2 column thì không làm gì hết, tránh crash trang web
      if (!activeWorkflow || !overWorkflow) return

      // Hành động kéo thả card giữa 2 column khác nhau
      if (oldWorkflowWhenDraggingCard.id !== overWorkflow.id) {
        moveCardBetweenDifferentWorkflows(
          overWorkflow,
          overCardId,
          active,
          over,
          activeWorkflow,
          activeDraggingCardId,
          activeDraggingCardData
        )
        const newCardIndex = overWorkflow?.cards?.findIndex(
          c => c.id === overCardId
        )
        try {
          const request = {
            MoveId: activeDraggingCardId,
            SpaceId: overWorkflow.id,
            NewPosition: newCardIndex
          }
          UpdateTaskCardPosition(request)
        } catch (error) {
          setOrderedWorkflows(orderedWorkflows)
        }
      } else {
        // Hành động kéo thả card trong cùng 1 cái column

        const oldCardIndex = oldWorkflowWhenDraggingCard?.cards?.findIndex(
          c => c.id === activeDragItemId
        )
        const newCardIndex = overWorkflow?.cards?.findIndex(
          c => c.id === overCardId
        )

        // Dùng arrayMove vì kéo card trong một cái column thì tương tự với logic kéo column trong một cái board content
        const dndOrderedCards = arrayMove(
          oldWorkflowWhenDraggingCard?.cards,
          oldCardIndex,
          newCardIndex
        )

        setOrderedWorkflows(prevWorkflows => {
          // Clone mảng OrderedWorkflowsState cũ ra một cái mới để xử lý data rồi return - cập nhật lại OrderedWorkflowsState mới
          const nextWorkflows = cloneDeep(prevWorkflows)

          // Tìm tới Workflow mà chúng ta đang thả
          const targetWorkflow = nextWorkflows.find(
            workflow => workflow.id === overWorkflow.id
          )

          // Cập nhật lại 2 giá trị mới là card và cardOrderIds trong cái targetColumn
          targetWorkflow.cards = dndOrderedCards
          try {
            const request = {
              MoveId: activeDraggingCardId,
              SpaceId: overWorkflow.id,
              NewPosition: newCardIndex
            }
            UpdateTaskCardPosition(request)
          } catch (error) {
            setOrderedWorkflows(orderedWorkflows)
          }
          // Trả về giá trị state mới (chuẩn vị trí)
          return nextWorkflows
        })
      }
    }
    if (
      activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.WORKFLOW &&
      active.id !== over.id
    ) {
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
      const request = {
        MoveId: active.id,
        SpaceId: board.id,
        NewPosition: newIndex
      }
      moveWorkflows(request)
    }
    setActiveDragItemId(null)
    setActiveDragItemType(null)
    setActiveDragItemData(null)
    setOldWorkflowWhenDraggingCard(null)
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

  const collisionDetectionStrategy = useCallback(
    // Trường hợp kéo column thì dùng thuật toán closestCorners là chuẩn nhất
    args => {
      if (activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.WORKFLOW) {
        return closestCorners({ ...args })
      }

      // Tìm các điểm giao nhau, va chạm, trả về một mảng các va chạm - intersections với con trỏ
      const pointerIntersections = pointerWithin(args)

      // Video 37.1: Nếu pointerIntersections là mảng rỗng, return luôn không làm gì hết
      // Fix triệt để cái bug flickering của thư viện Dnd-kit trong trường hợp sau:
      // - Kéo một cái card có image cover lớn và kéo lên phía trên cùng ra khỏi khu vực kéo thả
      if (!pointerIntersections?.length) return

      // Thuật toán phát hiện va chạm sẽ trả về một mảng các va chạm ở đây (không cần bước này nữa - video 37.1)
      // const intersections = !!pointerIntersections?.length ? pointerIntersections : rectIntersection(args)

      // Tìm overId đầu tiên trong đám intersection ở trên
      let overId = getFirstCollision(pointerIntersections, 'id')
      if (overId) {
        // Video 37: Đoạn này để fix cái vụ flickering nhé
        // Nếu cái over nó là column thì sẽ tìm tới cái cardId gần nhất bên trong khu vực va chạm đó dựa vào thuật toán phát hiện va chạm closestCenter hoặc closestCorners đều được. Tuy nhiên ở đây dùng closestCorners mình thấy mượt mà hơn
        const checkWorkflow = orderedWorkflows.find(
          workflow => workflow.id === overId
        )
        if (checkWorkflow) {
          overId = closestCorners({
            ...args,
            droppableContainers: args.droppableContainers.filter(container => {
              return (
                container.id !== overId &&
                checkWorkflow?.cardOrderIds?.includes(container.id)
              )
            })
          })[0]?.id
        }

        lastOverId.current = overId
        return [{ id: overId }]
      }

      // Nếu overId là null thì trả về mảng rỗng - tránh bug crash trang
      return lastOverId.current ? [{ id: lastOverId.current }] : []
    },
    [activeDragItemType, orderedWorkflows]
  )

  return (
    <DndContext
      onDragStart={handleDragStart}
      onDragOver={handleDragOver}
      onDragEnd={handleDragEnd}
      collisionDetection={collisionDetectionStrategy}
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
        <ListWorkflows
          workflows={orderedWorkflows}
          createNewWorkflow={createNewWorkflow}
        />
        <DragOverlay dropAnimation={customDropAnimation}>
          {!activeDragItemType && null}
          {activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.WORKFLOW && (
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
