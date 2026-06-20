export interface TaskDto {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  dueDate?: string;
  categoryId?: number;
  categoryName?: string;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  dueDate?: string;
  categoryId?: number;
}

export interface UpdateTaskDto {
  title: string;
  description: string;
  isCompleted: boolean;
  dueDate?: string;
  categoryId?: number;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}