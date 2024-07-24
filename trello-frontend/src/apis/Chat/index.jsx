import { instanceChatAPI } from '../Axios'

const END_POINT = {
  Chat: 'chat',
  GetChatByMembers: 'GetChatByMembers',
  Update: 'update'
}

const GetChatByMembers = userIds => {
  return instanceChatAPI.post(
    `${END_POINT.Chat}/${END_POINT.GetChatByMembers}`,
    userIds,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
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
