import Clock from '@public/clock.svg';
import Add from '@public/add.svg';

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
        <button className="flex items-center">
          <span className="text-black mr-2">Add time</span>
          <Add className="dark:invert h-3 w-3" />
        </button>
      </div>
    </div>
  );
}
