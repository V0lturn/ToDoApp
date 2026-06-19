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