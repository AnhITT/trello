import axios from 'axios'
import { API_ROOT, API_ROOT_CHAT, API_ROOT_UPLOAD } from '~/utils/constants'
import Cookies from 'js-cookie'

// Function to create axios instance with token interceptor
const createInstance = baseURL => {
  const instance = axios.create({
    baseURL
  })

  instance.interceptors.request.use(
    config => {
      const token = Cookies.get('token')
      if (token) {
        config.headers['Authorization'] = `Bearer ${token}`
      }
      return config
    },
    error => {
      return Promise.reject(error)
    }
  )

  instance.interceptors.response.use(
    response => {
      return response.data
    },
    error => {
      return Promise.reject(error)
    }
  )

  return instance
}

// Create instances for each API root
const instanceTrelloAPI = createInstance(API_ROOT)
const instanceChatAPI = createInstance(API_ROOT_CHAT)
const instanceUploadAPI = createInstance(API_ROOT_UPLOAD)

export { instanceTrelloAPI, instanceChatAPI, instanceUploadAPI }
