export type TaskPriority = 'Low' | 'Medium' | 'High';

type TaskPriorityMappingType = Record<number, TaskPriority>;
export const TaskPriorityMapping: TaskPriorityMappingType = {
  0: 'High',
  1: 'Medium',
  2: 'Low',
};

export const ReverseTaskPriorityMapping: Record<TaskPriority, number> = {
  High: 0,
  Medium: 1,
  Low: 2,
};

export type TaskStatus = 'Pending' | 'InProgress' | 'Completed';
export const TaskStatusMapping: Record<TaskStatus, number> = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
};

export const ReverseTaskStatusMapping: Record<number, TaskStatus> = {
  0: 'Pending',
  1: 'InProgress',
  2: 'Completed',
};

export interface Task {
  id: number | null;
  title: string | undefined;
  description: string | undefined;
  priority: TaskPriority | undefined;
  status: TaskStatus | undefined;
  tagIds: number[];
}
