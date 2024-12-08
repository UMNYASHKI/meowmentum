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

  const handleDelete = async (id: number) => {
    try {
      setTimeIntervals((prevIntervals) =>
        prevIntervals.filter((interval) => interval.id !== id)
      );
    } catch (error) {
      console.error('Error deleting interval:', error);
    }
  };

  const handleEdit = async (updatedInterval: ITimeInterval) => {
    setTimeIntervals((prevIntervals) =>
      prevIntervals.map((item) =>
        item.id === updatedInterval.id ? updatedInterval : item
      )
    );
  }

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
  onEdit
}: {
  interval: ITimeInterval;
  onDelete: (id: number) => void;
  onEdit: (interval: ITimeInterval) => void;
}) {
  const formattedDate = interval.date.toLocaleDateString('en-GB');
  const handleEdit = (id: number) => {

  };
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
          onClick={() => onDelete(interval.id)}
        >
          <Delete className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
}
