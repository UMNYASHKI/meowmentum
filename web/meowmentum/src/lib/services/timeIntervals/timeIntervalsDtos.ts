export interface AddTimeIntervalRequest {
  taskId: number;
  startTime: Date | null;
  spendedTime: number | null;
  description: string | null;
}

export interface UpdateTimeIntervalRequest {
  id: number;
  startTime: Date | null;
  endTime: Date | null;
  spendedTime: number | null;
  description: string | null;
}

export interface TimeIntervalResponse {
  id: number;
  startTime: Date | null;
  endTime: Date | null;
  description: string | null;
  taskId: number;
}
