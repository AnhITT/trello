import { instanceChatAPI } from '../Axios'

const END_POINT = {
  Chat: 'chat',
  GetChatByMembers: 'GetChatByMembers'
}

const GetAllPropertiesFromBoard = (idUser1, idUser2) => {
  return instanceChatAPI.get(
    `${END_POINT.Chat}/${END_POINT.GetChatByMembers}?idUser1=${idUser1}&idUser2=${idUser2}`
  )
}

export { GetAllPropertiesFromBoard }
