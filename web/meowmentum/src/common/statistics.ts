export type ReportType = 'completedTasks' | 'tagReport' | 'deadlineReport';

export const ReportViewNameByType: Record<ReportType, string> = {
  completedTasks: 'Completed Tasks',
  tagReport: 'Tag Report',
  deadlineReport: 'Deadline Report',
};

export const StatisticsDateFormat = 'yyyy-MM-dd';
