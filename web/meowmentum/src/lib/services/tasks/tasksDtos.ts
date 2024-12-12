import { ITag } from '@/common/tags';

export interface CreateTaskRequest {
  id: number | null;
  title: string;
  description: string;
  deadline: Date | undefined;
  priority: number | undefined;
  status: number | undefined;
  tagIds: number[];
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
  status: number | undefined;
  tags: TagPlain[];
}

export interface TaskFilterRequest {
  taskId: number | null;
  status: string[];
  tagIds: number[];
  priorities: string[];
}
