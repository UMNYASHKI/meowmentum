import { TaskPriority, TaskStatus } from '@/common/tasks';
export enum TaskStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  deadline: Date | undefined;
  priority: number | undefined;
  //priority: TaskPriority | undefined;
  status: TaskStatus | undefined;
  tagId: number | undefined;
  // todo: tags management  + status
export enum TaskPriority {
  High = 'High',
  Medium = 'Medium',
  Low = 'Low',
}

export interface TimeInterval {
  id: number;
  startTime: Date;
  endTime: Date;
  description: string;
}

export interface TaskResponse {
  id: number;
  title: string | null;
  description: string | null;
  createdAt: Date;
  deadline: Date | null;
  status: TaskStatus | null;
  priority: TaskPriority | null;
  tagId: number | null;
  tagName: string | null;
  timeSpent: TimeInterval[] | null;
export interface TaskFilterRequest {
  taskId: number | null;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
