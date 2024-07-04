import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import ThemeRoutes from '~/routers/index.jsx'
import CustomizedProgressBars from '~/components/Loader'
import { Suspense } from 'react'

const App = () => {
  return (
    <Router>
      <Suspense fallback={<CustomizedProgressBars />}>
        <Routes>
          {ThemeRoutes.map((route, index) => {
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
        </Routes>
      </Suspense>
    </Router>
  )
}

export default App
