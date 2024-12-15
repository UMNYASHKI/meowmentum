import Clock from '@public/clock.svg';
import Add from '@public/add.svg';
import { AddTime } from '@components/time-logs/addTimeModal';
import { ITimeInterval } from '@/common/timeIntervals';

interface TimeLogsHeaderProps {
  onAdd: (interval: ITimeInterval) => void;
  taskId: number | null;
}
export default function TimeLogsHeader({ taskId, onAdd }: TimeLogsHeaderProps) {
  return (
    <div className="flex justify-between items-center">
      <div className="flex flex-row items-center">
        <Clock className="h-5 w-5" />
        <span className="text-lg text-black ml-2 font-medium">
          Time Tracking
        </span>
      </div>
      <div className="flex flex-row items-center">
        <AddTime
          mode={'create'}
          taskId={taskId}
          interval={null}
          onSave={onAdd}
        />
      </div>
    </div>
  );
}
