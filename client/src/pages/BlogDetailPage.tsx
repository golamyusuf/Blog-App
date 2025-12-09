import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { blogApi } from '../api/blogApi';
import { useAuth } from '../contexts/AuthContext';
import { Blog } from '../types';
import { toast } from 'react-toastify';
import './BlogDetailPage.css';

const BlogDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [blog, setBlog] = useState<Blog | null>(null);
  const [loading, setLoading] = useState(true);
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (id) {
      loadBlog();
    }
  }, [id]);

  const loadBlog = async () => {
    try {
      const data = await blogApi.getBlogById(id!);
      setBlog(data);
    } catch (error) {
      toast.error('Failed to load blog');
      navigate('/');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this blog?')) {
      return;
    }

    try {
      await blogApi.deleteBlog(id!);
      toast.success('Blog deleted successfully');
      navigate('/');
    } catch (error) {
      toast.error('Failed to delete blog');
    }
  };

  const handleEdit = () => {
    navigate(`/edit-blog/${id}`);
  };

  if (loading) {
    return <div className="container loading-page">Loading...</div>;
  }

  if (!blog) {
    return <div className="container">Blog not found</div>;
  }

  const canEdit = user?.id === blog.userId;
  const canDelete = user?.id === blog.userId || isAdmin;

  return (
    <div className="blog-detail-page">
      <div className="container">
        <article className="blog-detail">
          <header className="blog-header">
            <h1 className="blog-title">{blog.title}</h1>
            
            <div className="blog-meta">
              <span className="blog-author">By {blog.username}</span>
              <span className="blog-date">
                {new Date(blog.createdAt).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </span>
              <span className="blog-views">👁️ {blog.viewCount} views</span>
            </div>

            {(canEdit || canDelete) && (
              <div className="blog-actions">
                {canEdit && (
                  <button onClick={handleEdit} className="btn-edit">
                    Edit
                  </button>
                )}
                {canDelete && (
                  <button onClick={handleDelete} className="btn-delete">
                    Delete
                  </button>
                )}
              </div>
            )}
          </header>

          {blog.mediaItems.length > 0 && (
            <div className="blog-media">
              {blog.mediaItems.map((media, index) => (
                <div key={index} className="media-item">
                  {media.type === 'Image' ? (
                    <img src={media.url} alt={media.caption || blog.title} />
                  ) : (
                    <video controls src={media.url} />
                  )}
                  {media.caption && <p className="media-caption">{media.caption}</p>}
                </div>
              ))}
            </div>
          )}

          <div className="blog-content" dangerouslySetInnerHTML={{ __html: blog.content }} />

          {blog.tags.length > 0 && (
            <div className="blog-tags">
              {blog.tags.map((tag, index) => (
                <span key={index} className="tag">#{tag}</span>
              ))}
            </div>
          )}
        </article>
      </div>
    </div>
  );
};

export default BlogDetailPage;
