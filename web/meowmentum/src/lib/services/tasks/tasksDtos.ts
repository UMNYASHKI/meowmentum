export enum TaskStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
}

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
}
