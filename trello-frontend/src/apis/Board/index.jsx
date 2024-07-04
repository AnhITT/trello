import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Board: 'board',
  GetAllPropertiesFromBoard: 'GetAllPropertiesFromBoard'
}

const GetAllPropertiesFromBoard = idBoard => {
  return instanceTrelloAPI.get(
    `${END_POINT.Board}/${END_POINT.GetAllPropertiesFromBoard}?idBoard=${idBoard}`
  )
}
export { GetAllPropertiesFromBoard }
