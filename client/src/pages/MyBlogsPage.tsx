import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { blogApi } from '../api/blogApi';
import { Blog } from '../types';
import { toast } from 'react-toastify';
import './MyBlogsPage.css';

const MyBlogsPage: React.FC = () => {
  const [blogs, setBlogs] = useState<Blog[]>([]);
  const [loading, setLoading] = useState(true);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const navigate = useNavigate();

  useEffect(() => {
    loadMyBlogs();
  }, [pageNumber]);

  const loadMyBlogs = async () => {
    try {
      setLoading(true);
      const response = await blogApi.getMyBlogs(pageNumber, 10);
      setBlogs(response.blogs);
      setTotalPages(response.totalPages);
    } catch (error) {
      toast.error('Failed to load your blogs');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this blog?')) {
      return;
    }

    try {
      await blogApi.deleteBlog(id);
      toast.success('Blog deleted successfully');
      loadMyBlogs();
    } catch (error) {
      toast.error('Failed to delete blog');
    }
  };

  if (loading) {
    return <div className="container loading-page">Loading your blogs...</div>;
  }

  return (
    <div className="my-blogs-page">
      <div className="container">
        <div className="page-header">
          <h1>My Blogs</h1>
          <button onClick={() => navigate('/create-blog')} className="btn-create">
            Create New Blog
          </button>
        </div>

        {blogs.length === 0 ? (
          <div className="no-blogs">
            <p>You haven't created any blogs yet.</p>
            <button onClick={() => navigate('/create-blog')} className="btn-create">
              Create Your First Blog
            </button>
          </div>
        ) : (
          <>
            <div className="blogs-table">
              <table>
                <thead>
                  <tr>
                    <th>Title</th>
                    <th>Status</th>
                    <th>Views</th>
                    <th>Created</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {blogs.map((blog) => (
                    <tr key={blog.id}>
                      <td className="blog-title-cell" onClick={() => navigate(`/blog/${blog.id}`)}>
                        {blog.title}
                      </td>
                      <td>
                        <span className={`status-badge ${blog.isPublished ? 'published' : 'draft'}`}>
                          {blog.isPublished ? 'Published' : 'Draft'}
                        </span>
                      </td>
                      <td>{blog.viewCount}</td>
                      <td>{new Date(blog.createdAt).toLocaleDateString()}</td>
                      <td className="actions-cell">
                        <button onClick={() => navigate(`/edit-blog/${blog.id}`)} className="btn-edit">
                          Edit
                        </button>
                        <button onClick={() => handleDelete(blog.id)} className="btn-delete">
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
          </>
        )}
      </div>
    </div>
  );
};

export default MyBlogsPage;
