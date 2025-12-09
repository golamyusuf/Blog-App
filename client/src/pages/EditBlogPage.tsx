import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { blogApi } from '../api/blogApi';
import { categoryApi } from '../api/categoryApi';
import { toast } from 'react-toastify';
import { MediaItem, Category } from '../types';
import './BlogFormPage.css';

const EditBlogPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [formData, setFormData] = useState({
    categoryId: '',
    title: '',
    content: '',
    summary: '',
    tags: '',
    isPublished: false
  });
  const [categories, setCategories] = useState<Category[]>([]);
  const [mediaItems, setMediaItems] = useState<MediaItem[]>([]);
  const [currentMedia, setCurrentMedia] = useState({ url: '', type: 'Image' as 'Image' | 'Video', caption: '' });
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const data = await categoryApi.getCategories(true);
        setCategories(data);
      } catch (error) {
        toast.error('Failed to load categories');
      }
    };
    loadCategories();
  }, []);

  useEffect(() => {
    if (id) {
      loadBlog();
    }
  }, [id]);

  const loadBlog = async () => {
    try {
      const blog = await blogApi.getBlogById(id!);
      setFormData({
        categoryId: blog.categoryId?.toString() || '',
        title: blog.title,
        content: blog.content,
        summary: blog.summary || '',
        tags: blog.tags.join(', '),
        isPublished: blog.isPublished
      });
      setMediaItems(blog.mediaItems);
    } catch (error) {
      toast.error('Failed to load blog');
      navigate('/my-blogs');
    } finally {
      setInitialLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
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
    setLoading(true);

    try {
      const tagsArray = formData.tags ? formData.tags.split(',').map(tag => tag.trim()).filter(tag => tag) : [];
      
      await blogApi.updateBlog(id!, {
        categoryId: formData.categoryId ? parseInt(formData.categoryId) : undefined,
        title: formData.title,
        content: formData.content,
        summary: formData.summary || undefined,
        tags: tagsArray,
        mediaItems: mediaItems,
        isPublished: formData.isPublished
      });

      toast.success('Blog updated successfully!');
      navigate('/my-blogs');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Failed to update blog');
    } finally {
      setLoading(false);
    }
  };

  if (initialLoading) {
    return <div className="container loading-page">Loading...</div>;
  }

  return (
    <div className="blog-form-page">
      <div className="container">
        <div className="blog-form-container">
          <h2 className="form-title">Edit Blog</h2>

          <form onSubmit={handleSubmit} className="blog-form">
            <div className="form-group">
              <label htmlFor="categoryId">Category</label>
              <select
                id="categoryId"
                name="categoryId"
                value={formData.categoryId}
                onChange={handleChange}
              >
                <option value="">-- Select Category (Optional) --</option>
                {categories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

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
              <label htmlFor="content">Content *</label>
              <textarea
                id="content"
                name="content"
                value={formData.content}
                onChange={handleChange}
                required
                placeholder="Write your blog content here..."
                rows={15}
              />
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
                Published
              </label>
            </div>

            <div className="form-actions">
              <button type="button" onClick={() => navigate(-1)} className="btn-cancel">
                Cancel
              </button>
              <button type="submit" className="btn-submit" disabled={loading}>
                {loading ? 'Updating...' : 'Update Blog'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default EditBlogPage;
