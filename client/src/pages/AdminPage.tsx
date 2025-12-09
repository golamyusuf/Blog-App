import React, { useState, useEffect } from 'react';
import { blogApi } from '../api/blogApi';
import { Blog } from '../types';
import { toast } from 'react-toastify';
import './AdminPage.css';

const AdminPage: React.FC = () => {
  const [blogs, setBlogs] = useState<Blog[]>([]);
  const [loading, setLoading] = useState(true);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    loadAllBlogs();
  }, [pageNumber]);

  const loadAllBlogs = async () => {
    try {
      setLoading(true);
      const response = await blogApi.getBlogs(pageNumber, 15);
      setBlogs(response.blogs);
      setTotalPages(response.totalPages);
    } catch (error) {
      toast.error('Failed to load blogs');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteBlog = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this blog? This action cannot be undone.')) {
      return;
    }

    try {
      await blogApi.deleteBlog(id);
      toast.success('Blog deleted successfully');
      loadAllBlogs();
    } catch (error) {
      toast.error('Failed to delete blog');
    }
  };

  if (loading) {
    return <div className="container loading-page">Loading blogs...</div>;
  }

  return (
    <div className="admin-page">
      <div className="container">
        <h1 className="page-title">Admin Dashboard</h1>
        <p className="page-subtitle">Manage all blog posts</p>

        <div className="admin-table">
          <table>
            <thead>
              <tr>
                <th>Title</th>
                <th>Author</th>
                <th>Status</th>
                <th>Views</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {blogs.map((blog) => (
                <tr key={blog.id}>
                  <td className="blog-title">{blog.title}</td>
                  <td>{blog.username}</td>
                  <td>
                    <span className={`status-badge ${blog.isPublished ? 'published' : 'draft'}`}>
                      {blog.isPublished ? 'Published' : 'Draft'}
                    </span>
                  </td>
                  <td>{blog.viewCount}</td>
                  <td>{new Date(blog.createdAt).toLocaleDateString()}</td>
                  <td>
                    <button onClick={() => handleDeleteBlog(blog.id)} className="btn-delete">
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {totalPages > 1 && (
          <div className="pagination">
            <button
              onClick={() => setPageNumber(p => Math.max(1, p - 1))}
              disabled={pageNumber === 1}
              className="pagination-btn"
            >
              Previous
            </button>
            <span className="page-info">
              Page {pageNumber} of {totalPages}
            </span>
            <button
              onClick={() => setPageNumber(p => Math.min(totalPages, p + 1))}
              disabled={pageNumber === totalPages}
              className="pagination-btn"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminPage;
