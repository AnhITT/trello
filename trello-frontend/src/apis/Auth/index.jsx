import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Auth: 'auth',
  Login: 'login',
  Register: 'register',
  VerifyEmail: 'verifyemail',
  ForgotPassword: 'forgotpassword',
  ConfirmOTPChangePassword: 'ConfirmOTPChangePassword'
}

const Login = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.Login}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}

const Register = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.Register}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}

const VerifyEmail = tokenEncode => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.VerifyEmail}?tokenEncode=${tokenEncode}`
  )
}

const ForgotPassword = email => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.ForgotPassword}?email=${email}`
  )
}

const ConfirmOTPChangePassword = request => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.ConfirmOTPChangePassword}`,
    request,
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  )
}

export {
  Login,
  Register,
  VerifyEmail,
  ForgotPassword,
  ConfirmOTPChangePassword
}
