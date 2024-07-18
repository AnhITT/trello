import { instanceChatAPI } from '../Axios'

const END_POINT = {
  Chat: 'chat',
  GetChatByMembers: 'GetChatByMembers',
  Update: 'update'
}

const GetChatByMembers = (idUser1, idUser2) => {
  return instanceChatAPI.get(
    `${END_POINT.Chat}/${END_POINT.GetChatByMembers}?idUser1=${idUser1}&idUser2=${idUser2}`
  )
}

const SendMessage = request => {
  return instanceChatAPI.patch(
    `${END_POINT.Chat}/${END_POINT.Update}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}

export { GetChatByMembers, SendMessage }
