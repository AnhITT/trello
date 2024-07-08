import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { PublicRoutes, PrivateRoutes, PrivateRoute } from '~/routers/index.jsx'
import CustomizedProgressBars from '~/components/Loader'
import { Suspense } from 'react'
import { AuthProvider } from '~/context/AuthProvider'

const App = () => {
  return (
    <Router>
      <AuthProvider>
        <Suspense fallback={<CustomizedProgressBars />}>
          <Routes>
            {PublicRoutes.map((route, index) => {
              if (route.children) {
                return (
                  <Route key={index} path={route.path} element={route.element}>
                    {route.children.map((child, idx) => (
                      <Route
                        key={idx}
                        path={child.path}
                        element={child.element}
                      />
                    ))}
                  </Route>
                )
              }
              return (
                <Route key={index} path={route.path} element={route.element} />
              )
            })}
            <Route element={<PrivateRoute />}>
              {PrivateRoutes.map((route, index) => {
                if (route.children) {
                  return (
                    <Route
                      key={index}
                      path={route.path}
                      element={route.element}
                    >
                      {route.children.map((child, idx) => (
                        <Route
                          key={idx}
                          path={child.path}
                          element={child.element}
                        />
                      ))}
                    </Route>
                  )
                }
                return (
                  <Route
                    key={index}
                    path={route.path}
                    element={route.element}
                  />
                )
              })}
            </Route>
          </Routes>
        </Suspense>
      </AuthProvider>
    </Router>
  )
}

export default App
