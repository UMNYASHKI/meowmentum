import { ITag } from '@/common/tags';
import { TimeIntervalResponse } from '@services/timeIntervals/timeIntervalsDtos';
import { TaskPriority, TaskStatus } from '@/common/tasks';

export interface CreateTaskRequest {
  id: number | null;
  title: string;
  description: string;
  deadline: Date | undefined;
  priority: number | undefined;
  status: number | undefined;
  tagIds: number[];
}

export interface TaskResponse {
  id: number;
  title: string | undefined;
  description: string | undefined;
  createdAt: Date;
  deadline: Date | undefined;
  status: number | undefined;
  priority: number | undefined;
  tags: TagPlain[];
  timeIntervals: TimeIntervalResponse[] | undefined;
}

export interface TagPlain extends ITag {
  id: number;
  name: string;
}

export interface TaskFilterRequest {
  taskId: number | undefined;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
