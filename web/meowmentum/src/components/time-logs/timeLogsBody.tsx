import { useState } from 'react';
import { ITimeInterval } from '@/common/timeIntervals';

import Delete from '@public/delete.svg';
import { AddTime } from '@components/time-logs/addTimeModal';

interface TimeLogsBodyProps {
  taskId: number | null;
  timeIntervals: ITimeInterval[];
  handleDelete: (id: number | null | undefined) => void;
  handleEdit: (interval: ITimeInterval) => void;
}
export default function TimeLogsBody({
  timeIntervals,
  handleDelete,
  handleEdit,
}: TimeLogsBodyProps) {
  return (
    <div className="mt-3">
      {timeIntervals.map((timeInterval) => (
        <TimeInterval
          key={timeInterval.id}
          interval={timeInterval}
          onDelete={handleDelete}
          onEdit={handleEdit}
        />
      ))}
    </div>
  );
}

function TimeInterval({
  interval,
  onDelete,
  onEdit,
}: {
  interval: ITimeInterval;
  onDelete: (id: number | null | undefined) => void;
  onEdit: (interval: ITimeInterval) => void;
}) {
  return (
    <div className="flex justify-between items-center py-2 border-b border-gray-200">
      <div className="flex items-center space-x-2">
        <div className="text-[#282828] text-sm">
          {interval.date != null
            ? interval.date.toLocaleDateString('en-GB')
            : 'Invalid Date'}
        </div>
        <span className="text-gray-400">—</span>
        <div className="text-[#282828] text-sm">Logged {interval.amount}</div>
      </div>

      <div className="flex items-center space-x-4">
        <AddTime
          mode={'edit'}
          taskId={interval.taskId}
          interval={interval}
          onSave={onEdit}
        />

        <button
          className="text-gray-400 hover:text-gray-600"
          aria-label="Delete"
          onClick={() => onDelete(interval.id)}
        >
          <Delete className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
}
