import Clock from '@public/clock.svg';
import Add from '@public/add.svg';
import { AddTime } from '@components/time-logs/addTimeModal';

export default function TimeLogsHeader() {
  return (
    <div className="flex justify-between items-center">
      <div className="flex flex-row items-center">
        <Clock className="h-5 w-5" />
        <span className="text-lg text-black ml-2 font-medium">
          Time Tracking
        </span>
      </div>
      <div className="flex flex-row items-center">
        <AddTime taskId={1} />
      </div>
    </div>
  );
}
