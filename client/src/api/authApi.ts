import axiosInstance from './axios';
import { LoginRequest, LoginResponse, RegisterRequest, User } from '../types';

export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post('/auth/login', data);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<User> => {
    const response = await axiosInstance.post('/auth/register', data);
    return response.data;
  },
};
