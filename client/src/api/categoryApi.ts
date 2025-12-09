import axiosInstance from './axios';
import { Category, CreateCategoryRequest } from '../types';

export const categoryApi = {
  getCategories: async (activeOnly = true): Promise<Category[]> => {
    const response = await axiosInstance.get(`/categories?activeOnly=${activeOnly}`);
    return response.data;
  },

  createCategory: async (data: CreateCategoryRequest): Promise<Category> => {
    const response = await axiosInstance.post('/categories', data);
    return response.data;
  },
};
