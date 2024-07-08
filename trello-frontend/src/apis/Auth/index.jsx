import { instanceTrelloAPI } from '../Axios'

const END_POINT = {
  Auth: 'auth',
  Login: 'login',
  Register: 'register',
  VerifyEmail: 'verifyemail',
  ForgotPassword: 'forgotpassword',
  ConfirmOTPChangePassword: 'ConfirmOTPChangePassword'
}

const LoginAPI = request => {
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

const RegisterAPI = request => {
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

const VerifyEmailAPI = tokenEncode => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.VerifyEmail}?tokenEncode=${tokenEncode}`
  )
}

const ForgotPasswordAPI = email => {
  return instanceTrelloAPI.post(
    `${END_POINT.Auth}/${END_POINT.ForgotPassword}?email=${email}`
  )
}

const ConfirmOTPChangePasswordAPI = request => {
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
  LoginAPI,
  RegisterAPI,
  VerifyEmailAPI,
  ForgotPasswordAPI,
  ConfirmOTPChangePasswordAPI
}
