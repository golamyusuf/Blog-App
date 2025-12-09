import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import Layout from './components/Layout/Layout';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import BlogDetailPage from './pages/BlogDetailPage';
import CreateBlogPage from './pages/CreateBlogPage';
import EditBlogPage from './pages/EditBlogPage';
import MyBlogsPage from './pages/MyBlogsPage';
import AdminPage from './pages/AdminPage';
import PrivateRoute from './components/PrivateRoute';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Layout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/blog/:id" element={<BlogDetailPage />} />
            <Route
              path="/create-blog"
              element={
                <PrivateRoute>
                  <CreateBlogPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/edit-blog/:id"
              element={
                <PrivateRoute>
                  <EditBlogPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/my-blogs"
              element={
                <PrivateRoute>
                  <MyBlogsPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/admin"
              element={
                <PrivateRoute requireAdmin>
                  <AdminPage />
                </PrivateRoute>
              }
            />
          </Routes>
        </Layout>
      </Router>
      <ToastContainer position="top-right" autoClose={3000} />
    </AuthProvider>
  );
}

export default App;
