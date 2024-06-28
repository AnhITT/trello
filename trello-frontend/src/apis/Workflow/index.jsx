import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Workflow: 'workflow',
  UpdateWorkflowPosition: 'UpdateWorkflowPosition',
  Create: 'create'
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
const AddWorkflow = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.Workflow}/${END_POINT.Create}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}
export { UpdateWorkflowPosition, AddWorkflow }
