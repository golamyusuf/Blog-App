import React from 'react';
import { Blog } from '../types';
import './BlogCard.css';

interface BlogCardProps {
  blog: Blog;
  onClick: () => void;
}

const BlogCard: React.FC<BlogCardProps> = ({ blog, onClick }) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getFirstImage = () => {
    return blog.mediaItems.find(item => item.type === 'Image')?.url;
  };

  return (
    <div className="blog-card" onClick={onClick}>
      {getFirstImage() && (
        <div className="blog-card-image">
          <img src={getFirstImage()} alt={blog.title} />
        </div>
      )}
      <div className="blog-card-content">
        <h3 className="blog-card-title">{blog.title}</h3>
        <p className="blog-card-summary">{blog.summary || blog.content.substring(0, 150) + '...'}</p>
        
        <div className="blog-card-footer">
          <div className="blog-card-meta">
            <span className="blog-author">By {blog.username}</span>
            <span className="blog-date">{formatDate(blog.createdAt)}</span>
          </div>
          <div className="blog-stats">
            <span>👁️ {blog.viewCount}</span>
          </div>
        </div>

        {blog.tags.length > 0 && (
          <div className="blog-tags">
            {blog.tags.slice(0, 3).map((tag, index) => (
              <span key={index} className="blog-tag">#{tag}</span>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default BlogCard;
