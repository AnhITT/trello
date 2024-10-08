import { createContext, useContext, useState, useEffect } from 'react'
import Cookies from 'js-cookie'
import {
  LoginAPI,
  RegisterAPI,
  VerifyEmailAPI,
  ForgotPasswordAPI,
  ConfirmOTPChangePasswordAPI
} from '~/apis/Auth'
import { toast } from 'react-toastify'
import { jwtDecode } from 'jwt-decode'

const AuthContext = createContext()

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [token, setToken] = useState(Cookies.get('token') || null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (token) {
      try {
        const decodedUser = jwtDecode(token)
        setUser(decodedUser)
      } catch (error) {
        setUser(null)
      }
    }
  }, [token])

  const login = async loginRequest => {
    setLoading(true)
    try {
      const response = await LoginAPI(loginRequest)
      if (response.statusCode === 200) {
        Cookies.set('token', response.data)
        const decodedUser = jwtDecode(response.data)
        setUser(decodedUser)
        window.location.href = '/board'
        return
      }
      return response.message
    } catch (error) {
      return error
    } finally {
      setLoading(false)
    }
  }

  const register = async registerRequest => {
    setLoading(true)
    try {
      const response = await RegisterAPI(registerRequest)
      if (response.statusCode === 201) {
        toast.success(
          'Registration successful, please confirm your gmail account!',
          {
            position: 'top-right',
            autoClose: false,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
            theme: 'light'
          }
        )
        return
      }
      return response.message
    } catch (error) {
      return error
    } finally {
      setLoading(false)
    }
  }

  const verifyEmail = async tokenEncode => {
    try {
      const response = await VerifyEmailAPI(tokenEncode)
      if (response.statusCode === 200) {
        return response.data
      }
      return response.message
    } catch (error) {
      return error
    }
  }
  const forgotPassword = async email => {
    setLoading(true)
    try {
      const response = await ForgotPasswordAPI(email)
      return response.message
    } catch (error) {
      return error
    } finally {
      setLoading(false)
    }
  }

  const confirmOTPChangePassword = async otpRequest => {
    setLoading(true)
    try {
      const response = await ConfirmOTPChangePasswordAPI(otpRequest)
      return response.message
    } catch (error) {
      return error
    } finally {
      setLoading(false)
    }
  }
  const logout = () => {
    Cookies.remove('token')
    setToken(null)
    return true
  }

  const isAuthenticated = () => {
    return !!token
  }

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        login,
        register,
        logout,
        verifyEmail,
        forgotPassword,
        confirmOTPChangePassword,
        isAuthenticated,
        loading
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export default AuthProvider
export const useAuth = () => useContext(AuthContext)
