import axios from 'axios'
import { API_ROOT } from '~/utils/constants'
import Cookies from 'js-cookie'

const instanceTrelloAPI = axios.create({
  baseURL: API_ROOT
})

// Add a request interceptor to attach the token to all requests
instanceTrelloAPI.interceptors.request.use(
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

instanceTrelloAPI.interceptors.response.use(
  response => {
    return response.data
  },
  error => {
    console.log(error)
    return Promise.reject(error)
  }
)

const instanceTrelloFileAPI = axios.create({
  baseURL: process.env.REACT_APP_TRELLOFILE_API
})

instanceTrelloFileAPI.interceptors.request.use(
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

instanceTrelloFileAPI.interceptors.response.use(
  response => {
    return response.data
  },
  error => {
    console.log(error)
    return Promise.reject(error)
  }
)

export { instanceTrelloAPI, instanceTrelloFileAPI }
