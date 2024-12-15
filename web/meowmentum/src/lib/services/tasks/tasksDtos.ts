import { ITag } from '@/common/tags';
import { TimeIntervalResponse } from '@services/timeIntervals/timeIntervalsDtos';

export interface CreateTaskRequest {
  id: number | null;
  title: string;
  description: string;
  deadline: Date | undefined;
  priority: number | undefined;
  //priority: TaskPriority | undefined;
  status: TaskStatus | undefined;
  tagId: number | undefined;
}

export interface TimeInterval {
  id: number;
  startTime: Date;
  endTime: Date;
  description: string;
}

export interface TaskResponse {
  id: number;
  title: string | undefined;
  description: string | undefined;
  createdAt: Date;
  deadline: Date | undefined;
  status: TaskStatus | undefined;
  priority: TaskPriority | undefined;
  tagId: number | undefined;
  tagName: string | undefined;
  timeSpent: TimeInterval[] | undefined;
}

export interface TaskFilterRequest {
  taskId: number | undefined;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
