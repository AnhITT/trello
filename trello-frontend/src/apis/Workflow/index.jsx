import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Workflow: 'workflow',
  UpdateWorkflowPosition: 'UpdateWorkflowPosition',
  Create: 'create',
  Delete: 'delete'
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

const DeleteWorkflow = idWorkflow => {
  return instanceTrelloAPI.delete(
    `${END_POINT.Workflow}/${END_POINT.Delete}?idWorkflow=${idWorkflow}`
  )
}
export { UpdateWorkflowPosition, AddWorkflow, DeleteWorkflow }
