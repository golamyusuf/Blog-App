import axiosInstance from './axios';
import { Blog, BlogListResponse, CreateBlogRequest, UpdateBlogRequest } from '../types';

export const blogApi = {
  getBlogs: async (pageNumber = 1, pageSize = 10, searchTerm?: string, categoryId?: number): Promise<BlogListResponse> => {
    const params = new URLSearchParams();
    params.append('pageNumber', pageNumber.toString());
    params.append('pageSize', pageSize.toString());
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (categoryId) params.append('categoryId', categoryId.toString());
    
    const response = await axiosInstance.get(`/blogs?${params.toString()}`);
    return response.data;
  },

  getBlogById: async (id: string): Promise<Blog> => {
    const response = await axiosInstance.get(`/blogs/${id}`);
    return response.data;
  },

  createBlog: async (data: CreateBlogRequest): Promise<Blog> => {
    const response = await axiosInstance.post('/blogs', data);
    return response.data;
  },

  updateBlog: async (id: string, data: UpdateBlogRequest): Promise<Blog> => {
    const response = await axiosInstance.put(`/blogs/${id}`, data);
    return response.data;
  },

  deleteBlog: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/blogs/${id}`);
  },

  getMyBlogs: async (pageNumber = 1, pageSize = 10): Promise<BlogListResponse> => {
    const response = await axiosInstance.get(`/blogs/my-blogs?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  },
};
