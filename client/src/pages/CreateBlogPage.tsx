import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { blogApi } from '../api/blogApi';
import { toast } from 'react-toastify';
import { MediaItem } from '../types';
import './BlogFormPage.css';

const CreateBlogPage: React.FC = () => {
  const [formData, setFormData] = useState({
    title: '',
    content: '',
    summary: '',
    tags: '',
    isPublished: true
  });
  const [mediaItems, setMediaItems] = useState<MediaItem[]>([]);
  const [currentMedia, setCurrentMedia] = useState({ url: '', type: 'Image' as 'Image' | 'Video', caption: '' });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleAddMedia = () => {
    if (!currentMedia.url) {
      toast.error('Please enter a media URL');
      return;
    }

    setMediaItems([...mediaItems, { ...currentMedia, order: mediaItems.length }]);
    setCurrentMedia({ url: '', type: 'Image', caption: '' });
  };

  const handleRemoveMedia = (index: number) => {
    setMediaItems(mediaItems.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Client-side validation
    if (formData.title.length > 200) {
      toast.error('Title cannot exceed 200 characters');
      return;
    }
    
    if (formData.content.length < 50) {
      toast.error('Content must be at least 50 characters');
      return;
    }
    
    if (formData.summary && formData.summary.length > 500) {
      toast.error('Summary cannot exceed 500 characters');
      return;
    }
    
    setLoading(true);

    try {
      const tagsArray = formData.tags ? formData.tags.split(',').map(tag => tag.trim()).filter(tag => tag) : [];
      
      if (tagsArray.length > 10) {
        toast.error('Maximum 10 tags allowed');
        setLoading(false);
        return;
      }
      
      await blogApi.createBlog({
        title: formData.title,
        content: formData.content,
        summary: formData.summary || undefined,
        tags: tagsArray,
        mediaItems: mediaItems,
        isPublished: formData.isPublished
      });

      toast.success('Blog created successfully!');
      navigate('/my-blogs');
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || error.response?.data?.errors || 'Failed to create blog';
      toast.error(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="blog-form-page">
      <div className="container">
        <div className="blog-form-container">
          <h2 className="form-title">Create New Blog</h2>

          <form onSubmit={handleSubmit} className="blog-form">
            <div className="form-group">
              <label htmlFor="title">Title *</label>
              <input
                type="text"
                id="title"
                name="title"
                value={formData.title}
                onChange={handleChange}
                required
                placeholder="Enter blog title"
              />
            </div>

            <div className="form-group">
              <label htmlFor="summary">Summary</label>
              <input
                type="text"
                id="summary"
                name="summary"
                value={formData.summary}
                onChange={handleChange}
                placeholder="Brief summary of your blog"
                maxLength={500}
              />
            </div>

            <div className="form-group">
              <label htmlFor="content">Content * (minimum 50 characters)</label>
              <textarea
                id="content"
                name="content"
                value={formData.content}
                onChange={handleChange}
                required
                placeholder="Write your blog content here... (minimum 50 characters)"
                rows={15}
              />
              <small style={{ color: formData.content.length < 50 ? '#e74c3c' : '#95a5a6' }}>
                {formData.content.length} / 50 characters minimum
              </small>
            </div>

            <div className="form-group">
              <label htmlFor="tags">Tags (comma-separated)</label>
              <input
                type="text"
                id="tags"
                name="tags"
                value={formData.tags}
                onChange={handleChange}
                placeholder="e.g., technology, programming, tutorial"
              />
            </div>

            <div className="media-section">
              <h3>Add Media</h3>
              <div className="media-inputs">
                <input
                  type="url"
                  value={currentMedia.url}
                  onChange={(e) => setCurrentMedia({ ...currentMedia, url: e.target.value })}
                  placeholder="Media URL"
                />
                <select
                  value={currentMedia.type}
                  onChange={(e) => setCurrentMedia({ ...currentMedia, type: e.target.value as 'Image' | 'Video' })}
                >
                  <option value="Image">Image</option>
                  <option value="Video">Video</option>
                </select>
                <input
                  type="text"
                  value={currentMedia.caption}
                  onChange={(e) => setCurrentMedia({ ...currentMedia, caption: e.target.value })}
                  placeholder="Caption (optional)"
                />
                <button type="button" onClick={handleAddMedia} className="btn-add-media">
                  Add
                </button>
              </div>

              {mediaItems.length > 0 && (
                <div className="media-list">
                  <h4>Added Media:</h4>
                  {mediaItems.map((media, index) => (
                    <div key={index} className="media-item-preview">
                      <span>{media.type}: {media.url.substring(0, 50)}...</span>
                      <button type="button" onClick={() => handleRemoveMedia(index)} className="btn-remove">
                        Remove
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="form-group checkbox-group">
              <label>
                <input
                  type="checkbox"
                  checked={formData.isPublished}
                  onChange={(e) => setFormData({ ...formData, isPublished: e.target.checked })}
                />
                Publish immediately
              </label>
            </div>

            <div className="form-actions">
              <button type="button" onClick={() => navigate(-1)} className="btn-cancel">
                Cancel
              </button>
              <button type="submit" className="btn-submit" disabled={loading}>
                {loading ? 'Creating...' : 'Create Blog'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default CreateBlogPage;
