export type TaskPriority = 'Low' | 'Medium' | 'High';

type TaskPriorityMappingType = Record<number, TaskPriority>;
export const TaskPriorityMapping: TaskPriorityMappingType = {
  0: 'High',
  1: 'Medium',
  2: 'Low',
};
export type TaskStatus = 'Pending' | 'InProgress' | 'Completed';
