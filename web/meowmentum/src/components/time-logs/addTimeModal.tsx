import Add from '@public/add.svg';
import { useEffect, useState } from 'react';
import Clock from '@public/clock.svg';
import Calendar from '@public/calendar.svg';
import { ITimeInterval } from '@/common/timeIntervals';
import EditSimple from '@public/edit-simple.svg';

type Mode = 'edit' | 'create';

interface AddTimeProps {
  mode: Mode;
  taskId: number | null;
  interval: ITimeInterval | null;
  onSave: (interval: ITimeInterval) => void;
}

export function AddTime({ mode, taskId, interval, onSave }: AddTimeProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [time, setTime] = useState('');
  const [date, setDate] = useState('');

  useEffect(() => {
    if (interval && mode === 'edit') {
      setTime(interval.amount ?? '');
      setDate(interval.date ? interval.date.toISOString().split('T')[0] : '');
    } else {
      setTime('');
      setDate('');
    }
  }, [interval, mode]);

  const onOpen = () => setIsOpen(true);

  const onClose = () => {
    if (!time || !date) {
      alert('Please provide both time and date.');
      return;
    }

    const updatedInterval: ITimeInterval = {
      id: taskId ?? interval?.id ?? Date.now(),
      amount: time,
      date: new Date(date),
    };

    onSave(updatedInterval);
    setIsOpen(false);
  };

  return (
    <div className="relative">
      <button className="flex items-center space-x-1" onClick={onOpen}>
        {mode === 'create' ? (
          <>
            <span className="text-black">Add time</span>
            <Add className="dark:invert h-4 w-4" />
          </>
        ) : (
          <EditSimple className="w-5 h-5" />
        )}
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 bg-white shadow-lg rounded-lg p-4 w-80 z-[100]">
          <div className="space-y-2">
            {/* Time and Date Inputs */}
            <div className="grid grid-cols-2 gap-4">
              {/* Time Input */}
              <div className="flex flex-col">
                <div className="flex items-center space-x-2">
                  <Clock className="h-4 w-4" />
                  <span className="text-gray-500 text-sm font-medium">
                    Time
                  </span>
                </div>
                <input
                  type="text"
                  placeholder="0h"
                  value={time}
                  onChange={(e) => setTime(e.target.value)}
                  className="border border-[#E5E5E5] bg-[#FAFAFA] rounded-lg mt-2 px-2 py-1 text-sm text-black"
                />
              </div>

              <div className="flex flex-col">
                <div className="flex items-center space-x-2">
                  <Calendar className="dark:invert h-4 w-4" />
                  <span className="text-gray-500 text-sm font-medium">
                    Date
                  </span>
                </div>
                <input
                  type="date"
                  value={date}
                  onChange={(e) => setDate(e.target.value)}
                  className="border border-[#E5E5E5] bg-[#FAFAFA] rounded-lg mt-2 px-2 py-1 text-sm text-black"
                />
              </div>
            </div>

            <div className="flex justify-end">
              <button
                className="bg-[#676A6E] text-white text-sm px-4 py-2 rounded-lg hover:bg-gray-800"
                onClick={onClose}
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
