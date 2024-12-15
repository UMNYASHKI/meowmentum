export const taskPriorities = ['Low', 'Medium', 'High'] as const;

export type TaskPriority = (typeof taskPriorities)[number];

type TaskPriorityMappingType = Record<number, TaskPriority>;
export const TaskPriorityMapping: TaskPriorityMappingType = {
  0: 'High',
  1: 'Medium',
  2: 'Low',
};

type ReversedTaskPriorityMappingType = Record<TaskPriority, number>;
export const ReversedTaskPriorityMapping: ReversedTaskPriorityMappingType = {
  High: 0,
  Medium: 1,
  Low: 2,
};

export const taskStatuses = ['Pending', 'InProgress', 'Completed'] as const;

export type TaskStatus = (typeof taskStatuses)[number];
