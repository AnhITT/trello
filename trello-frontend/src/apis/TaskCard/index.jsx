import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  TaskCard: 'taskcard',
  UpdateTaskCardPosition: 'UpdateTaskCardPosition',
  Create: 'create'
}
const AddTaskCard = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.TaskCard}/${END_POINT.Create}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}
const UpdateTaskCardPosition = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.TaskCard}/${END_POINT.UpdateTaskCardPosition}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}
export { UpdateTaskCardPosition, AddTaskCard }
