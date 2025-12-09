export interface User {
  id: number;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  profileImageUrl?: string;
  roles: string[];
  createdAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface Blog {
  id: string;
  userId: number;
  username: string;
  title: string;
  content: string;
  summary?: string;
  tags: string[];
  mediaItems: MediaItem[];
  viewCount: number;
  isPublished: boolean;
  createdAt: string;
  updatedAt?: string;
  publishedAt?: string;
}

export interface MediaItem {
  url: string;
  type: 'Image' | 'Video';
  caption?: string;
  order: number;
}

export interface CreateBlogRequest {
  title: string;
  content: string;
  summary?: string;
  tags: string[];
  mediaItems: MediaItem[];
  isPublished: boolean;
}

export interface UpdateBlogRequest extends CreateBlogRequest {}

export interface BlogListResponse {
  blogs: Blog[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
