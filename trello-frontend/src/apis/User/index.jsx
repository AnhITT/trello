import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  User: 'user',
  GetAllUserAPI: 'getall'
}

const GetAllUserAPI = () => {
  return instanceTrelloAPI.get(`${END_POINT.User}/${END_POINT.GetAllUserAPI}`)
}
export { GetAllUserAPI }
