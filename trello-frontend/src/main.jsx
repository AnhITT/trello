import React from 'react'
import ReactDOM from 'react-dom/client'
import App from '~/App.jsx'
import theme from '~/theme'
import { ToastContainer } from 'react-toastify'
import CssBaseline from '@mui/material/CssBaseline'
import 'react-toastify/dist/ReactToastify.css'
import { Experimental_CssVarsProvider as CssVarsProvider } from '@mui/material/styles'

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <CssVarsProvider theme={theme}>
      <CssBaseline />
      <App />
      <ToastContainer />
    </CssVarsProvider>
  </React.StrictMode>
)
