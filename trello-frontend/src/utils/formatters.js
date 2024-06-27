export const capitalizeFirstLetter = val => {
  if (!val) return ''
  return `${val.charAt(0).toUpperCase()}${val.slice(1)}`
}
export const generatePlaceholderCard = workflow => {
  return {
    id: `${workflow.id}-placeholder-card`,
    workflowId: workflow.id,
    FE_PlaceholderCard: true
  }
}
