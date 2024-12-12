import { ModalBody } from '@nextui-org/modal';
import TimeLogsHeader from '@components/time-logs/timeLogsHeader';
import TimeLogsBody from '@components/time-logs/timeLogsBody';
import { useEffect, useState } from 'react';
import { ITimeInterval } from '@/common/timeIntervals';
import {
  useAddIntervalMutation,
  useLazyDeleteIntervalQuery,
  useLazyGetAllTaskIntervalsQuery,
  useUpdateIntervalMutation,
} from '@services/timeIntervals/timeIntervalsApi';
import {
  AddTimeIntervalRequest,
  UpdateTimeIntervalRequest,
} from '@services/timeIntervals/timeIntervalsDtos';
import { useSetError } from '@utils/popUpsManager';
import { parseTimeToMinutes } from '@utils/timeHelpers';
import { transformTimeIntervals } from '@utils/timeIntervalsHelpers';

interface TimeLogsProps {
  taskId: number | null;
}
export default function TimeLogs({ taskId }: TimeLogsProps) {
  const setError = useSetError();
  const [timeIntervals, setTimeIntervals] = useState<ITimeInterval[]>([]);
  const [triggerDelete] = useLazyDeleteIntervalQuery();
  const [triggerUpdate] = useUpdateIntervalMutation();
  const [triggerAdd] = useAddIntervalMutation();
  const [triggerGetIntervals] = useLazyGetAllTaskIntervalsQuery();

  useEffect(() => {
    if (!taskId) {
      return;
    }
    try {
      triggerGetIntervals(taskId)
        .unwrap()
        .then((r) => {
          setTimeIntervals(transformTimeIntervals(r));
        });
    } catch (error) {
      setError('Failed to get time intervals');
      return;
    }
  }, [taskId]);

  const handleDelete = async (id: number | undefined | null) => {
    if (!id) {
      setError('Something went wrong');
      return;
    }
    try {
      await triggerDelete(id);
      setTimeIntervals((prevIntervals) =>
        prevIntervals.filter((interval) => interval.id !== id)
      );
    } catch (error) {
      console.error('Error deleting interval:', error);
    }
  };

  const handleEdit = async (updatedInterval: ITimeInterval) => {
    if (updatedInterval.id == null) return;

    const request: UpdateTimeIntervalRequest = {
      id: updatedInterval.id,
      startTime: updatedInterval.date,
      endTime: null,
      spendedTime: parseTimeToMinutes(updatedInterval.amount),
      description: null,
    };

    try {
      await triggerUpdate(request);
      setTimeIntervals((prevIntervals) =>
        prevIntervals.map((item) =>
          item.id === updatedInterval.id ? updatedInterval : item
        )
      );
    } catch (error) {
      setError('Failed to update time interval');
    }
  };

  const handleAdd = async (newInterval: ITimeInterval) => {
    if (!taskId) {
      setError('Something went wrong');
      return;
    }
    const request: AddTimeIntervalRequest = {
      description: null,
      spendedTime: parseTimeToMinutes(newInterval.amount),
      startTime: newInterval.date,
      taskId: taskId,
    };
    try {
      const id = await triggerAdd(request).unwrap();
      newInterval.id = id;
      setTimeIntervals((prevIntervals) => [...prevIntervals, newInterval]);
    } catch (error) {
      setError('Failed to add time interval');
    }
  };

  return (
    <ModalBody className="mt-4">
      <TimeLogsHeader onAdd={handleAdd} taskId={taskId} />
      <TimeLogsBody
        taskId={taskId}
        timeIntervals={timeIntervals}
        handleDelete={handleDelete}
        handleEdit={handleEdit}
      />
    </ModalBody>
  );
}
