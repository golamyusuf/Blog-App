import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { blogApi } from '../api/blogApi';
import { categoryApi } from '../api/categoryApi';
import { Blog, Category } from '../types';
import BlogCard from '../components/BlogCard';
import './HomePage.css';

const HomePage: React.FC = () => {
  const [blogs, setBlogs] = useState<Blog[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const navigate = useNavigate();

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const data = await categoryApi.getCategories(true);
        setCategories(data);
      } catch (error) {
        console.error('Failed to load categories', error);
      }
    };
    loadCategories();
  }, []);

  useEffect(() => {
    loadBlogs();
  }, [pageNumber, searchTerm, selectedCategoryId]);

  const loadBlogs = async () => {
    try {
      setLoading(true);
      const response = await blogApi.getBlogs(pageNumber, 12, searchTerm, selectedCategoryId ?? undefined);
      setBlogs(response.blogs);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error('Failed to load blogs', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPageNumber(1);
    loadBlogs();
  };

  return (
    <div className="home-page">
      <div className="hero-section">
        <div className="container">
          <h1 className="hero-title">Welcome to Our Blog</h1>
          <p className="hero-subtitle">Discover stories, thinking, and expertise from writers on any topic.</p>
          
          <form onSubmit={handleSearch} className="search-form">
            <input
              type="text"
              placeholder="Search blogs..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="search-input"
            />
            <button type="submit" className="search-button">Search</button>
          </form>
        </div>
      </div>

      <div className="container">
        {categories.length > 0 && (
          <div className="category-filter">
            <button 
              className={`category-chip ${selectedCategoryId === null ? 'active' : ''}`}
              onClick={() => {
                setSelectedCategoryId(null);
                setPageNumber(1);
              }}
            >
              All Categories
            </button>
            {categories.map(category => (
              <button
                key={category.id}
                className={`category-chip ${selectedCategoryId === category.id ? 'active' : ''}`}
                onClick={() => {
                  setSelectedCategoryId(category.id);
                  setPageNumber(1);
                }}
              >
                {category.name}
              </button>
            ))}
          </div>
        )}

        {loading ? (
          <div className="loading">Loading blogs...</div>
        ) : blogs.length === 0 ? (
          <div className="no-blogs">No blogs found</div>
        ) : (
          <>
            <div className="blogs-grid">
              {blogs.map((blog) => (
                <BlogCard key={blog.id} blog={blog} onClick={() => navigate(`/blog/${blog.id}`)} />
              ))}
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

export default HomePage;
