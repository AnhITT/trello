import { instanceChatAPI } from '../Axios'

const END_POINT = {
  Chat: 'chat',
  GetChatByMembers: 'GetChatByMembers',
  GetChatByMe: 'GetChatByMe',
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

const GetChatByMe = () => {
  return instanceChatAPI.get(`${END_POINT.Chat}/${END_POINT.GetChatByMe}`)
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

export { GetChatByMembers, SendMessage, GetChatByMe }
