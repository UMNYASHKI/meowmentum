import { ModalBody } from '@nextui-org/modal';
import TimeLogsHeader from '@components/time-logs/timeLogsHeader';
import TimeLogsBody from '@components/time-logs/timeLogsBody';
import { useState } from 'react';
import { ITimeInterval } from '@/common/timeIntervals';

export default function TimeLogs() {
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
  };

  const handleAdd = async (newInterval: ITimeInterval) => {
    // call to api and obtain new id
    setTimeIntervals((prevIntervals) => [...prevIntervals, newInterval]);
  };

  return (
    <ModalBody className="mt-4">
      <TimeLogsHeader onAdd={handleAdd} />
      <TimeLogsBody
        timeIntervals={timeIntervals}
        handleDelete={handleDelete}
        handleEdit={handleEdit}
      />
    </ModalBody>
  );
}
