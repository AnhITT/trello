import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Board: 'board',
  GetAllProptiesFromBoard: 'GetAllProptiesFromBoard'
}

const GetAllProptiesFromBoard = idBoard => {
  return instanceTrelloAPI.get(
    `${END_POINT.Board}/${END_POINT.GetAllProptiesFromBoard}?idBoard=${idBoard}`
  )
}
export { GetAllProptiesFromBoard }
