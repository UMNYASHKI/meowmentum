import { TaskPriority, TaskStatus } from '@/common/tasks';

export interface CreateTaskRequest {
  title: string;
  description: string;
  deadline: Date | undefined;
  priority: number | undefined;
  //priority: TaskPriority | undefined;
  status: TaskStatus | undefined;
  tagId: number | undefined;
  // todo: tags management  + status
}

export interface TaskResponse {
  id: number;
  title: string;
  description: string;
  deadline: Date;
  priority: number | undefined;
}

export interface TaskFilterRequest {
  taskId: number | null;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
