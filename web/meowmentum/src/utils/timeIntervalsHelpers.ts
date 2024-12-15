import { calculateDuration } from '@utils/timeHelpers';

export interface TimeIntervalResponse {
  id: number;
  startTime: Date | null;
  endTime: Date | null;
  description: string | null;
  taskId: number;
}

export interface ITimeInterval {
  id: number;
  taskId: number;
  date: Date | null;
  amount: string;
}

export function transformTimeIntervals(
  intervals: TimeIntervalResponse[] | undefined
): ITimeInterval[] {
  if (intervals === undefined) {
    return [];
  }

  return intervals.map((interval) => {
    const { id, taskId, startTime, endTime } = interval;
    const startDate = startTime != null ? new Date(startTime) : null;
    const endDate = endTime != null ? new Date(endTime) : null;
    const duration = calculateDuration(startDate, endDate);

    return {
      id,
      taskId,
      date: startDate,
      amount: duration,
    };
  });
}
