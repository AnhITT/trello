import ReactDOM from 'react-dom/client'
import App from '~/App.jsx'
import theme from '~/theme'
import { ToastContainer } from 'react-toastify'
import CssBaseline from '@mui/material/CssBaseline'
import 'react-toastify/dist/ReactToastify.css'
import { Experimental_CssVarsProvider as CssVarsProvider } from '@mui/material/styles'
import { ConfirmProvider } from 'material-ui-confirm' // Cấu hình Mui Dialog

ReactDOM.createRoot(document.getElementById('root')).render(
  <CssVarsProvider theme={theme}>
    <ConfirmProvider
      defaultOptions={{
        dialogProps: {
          maxWidth: 'xs'
        },
        confirmationButtonProps: {
          color: 'secondary',
          variant: 'outlined'
        },
        cancellationButtonProps: {
          color: 'inherit'
        },
        allowClose: false,
        buttonOrder: ['confirm', 'cancel']
      }}
    >
      <CssBaseline />
      <App />
      <ToastContainer position="bottom-left" theme="colored" />
    </ConfirmProvider>
  </CssVarsProvider>
)
