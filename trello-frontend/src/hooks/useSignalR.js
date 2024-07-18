import { useEffect, useState } from 'react'
import * as signalR from '@microsoft/signalr'

const useSignalR = url => {
  const [connection, setConnection] = useState(null)
  const [connected, setConnected] = useState(false)

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .build()

    setConnection(newConnection)
  }, [url])

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          setConnected(true)
          console.log('Connected to SignalR Hub')
        })
        .catch(err => console.log('Error while establishing connection: ', err))

      connection.onclose(() => setConnected(false))
    }
  }, [connection])

  return { connection, connected }
}

export default useSignalR
