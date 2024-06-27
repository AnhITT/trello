import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Workflow: 'workflow',
  UpdateWorkflowPosition: 'UpdateWorkflowPosition'
}

const UpdateWorkflowPosition = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.Workflow}/${END_POINT.UpdateWorkflowPosition}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}
export { UpdateWorkflowPosition }
