import { useState } from 'react';
import { ITimeInterval } from '@/common/timeIntervals';
import EditSimple from '@public/edit-simple.svg';
import Delete from '@public/delete.svg';

export default function TimeLogsBody() {
  // todo: fetch task intervals
  const [timeIntervals, setTimeIntervals] = useState<ITimeInterval[]>([
    {
      id: 1,
      date: new Date('2024-11-17'),
      amount: '1h',
    },
    {
      id: 2,
      date: new Date('2024-11-18'),
      amount: '2h',
    },
    {
      id: 3,
      date: new Date('2024-11-19'),
      amount: '45m',
    },
  ]);

  return (
    <div className="mt-3">
      {timeIntervals.map((timeInterval) => (
        <TimeInterval key={timeInterval.id} interval={timeInterval} />
      ))}
    </div>
  );
}

function handleEdit(id: number) {}

function handleDelete(id: number) {}

function TimeInterval({ interval }: { interval: ITimeInterval }) {
  const formattedDate = interval.date.toLocaleDateString('en-GB');
  return (
    <div className="flex justify-between items-center py-2 border-b border-gray-200">
      <div className="flex items-center space-x-2">
        <div className="text-[#282828] text-sm">{formattedDate}</div>
        <span className="text-gray-400">—</span>
        <div className="text-[#282828] text-sm">Logged {interval.amount}</div>
      </div>

      <div className="flex items-center space-x-4">
        <button
          className="text-gray-400 hover:text-gray-600"
          aria-label="Edit"
          onClick={() => handleEdit(interval.id)}
        >
          <EditSimple className="w-5 h-5" />
        </button>

        <button
          className="text-gray-400 hover:text-gray-600"
          aria-label="Delete"
          onClick={() => handleDelete(interval.id)}
        >
          <Delete className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
}
