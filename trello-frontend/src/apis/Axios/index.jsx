import axios from 'axios'
import { API_ROOT } from '~/utils/constants'

const instanceTrelloAPI = axios.create({
  baseURL: API_ROOT
})

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
