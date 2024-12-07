import { TaskStatus } from '@/common/tasks';
import { ITag } from '@/common/tags';

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

export interface TagPlain extends ITag {
  id: number;
  name: string;
}

export interface TaskResponse {
  id: number;
  title: string;
  description: string;
  deadline: Date;
  priority: number | undefined;
  tags: TagPlain[];
}

export interface TaskFilterRequest {
  taskId: number | null;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
