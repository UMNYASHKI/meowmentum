import { TaskPriority, TaskStatus } from '@services/tasks/tasksDtos';
import More from '../../../public/more.svg';
import Today from '@public/today.svg';
import { twMerge } from 'tailwind-merge';

interface TaskShortViewProps {
  title: string;
  deadline: Date;
  status: TaskStatus;
  priority: TaskPriority;
}

interface TypeClasses<T> {
  type: T;
  className: string;
  placeholder: string;
}

const statusClasses: TypeClasses<TaskStatus>[] = [
  {
    type: TaskStatus.Pending,
    className: 'border-[#C9C9C9] bg-[#EBEBEB] text-[#959595]',
    placeholder: 'Not Started',
  },
  {
    type: TaskStatus.InProgress,
    className: 'border-[#F8D255] bg-[#FFF8D4] text-[#E5AC09]',
    placeholder: 'In Progress',
  },
  {
    type: TaskStatus.Completed,
    className: 'border-[#81D164] bg-[#EBFFDF] text-[#41B52D]',
    placeholder: 'Done',
  },
];
const priorityClasses: TypeClasses<TaskPriority>[] = [
  {
    type: TaskPriority.Low,
    className: 'border-[#81D164] bg-[#EBFFDF] text-[#41B52D]',
    placeholder: 'P2',
  },
  {
    type: TaskPriority.Medium,
    className: 'border-[#F8D255] bg-[#FFF8D4] text-[#E5AC09]',
    placeholder: 'P1',
  },
  {
    type: TaskPriority.High,
    className: 'border-[#F2B8B8] bg-[#FFE8E8] text-[#DD6969]',
    placeholder: 'P0',
  },
];

export default function TaskShortView({
  props,
}: {
  props: TaskShortViewProps;
}) {
  const statusClass: TypeClasses<TaskStatus> =
    statusClasses.find((x) => x.type === props.status) ?? statusClasses[0];
  const priorityClass: TypeClasses<TaskPriority> =
    priorityClasses.find((x) => x.type === props.priority) ??
    priorityClasses[0];
  return (
    <tr className="h-[42px] flex flex-row w-full justify-between">
      <th className="border w-[42px]"> </th>
      <th className="text-left min-w-[160px] max-w-[220px] overflow-auto border pl-[12px] pr-[12px] text-black text-[20px] font-[400]">
        {props.title}
      </th>
      <th className="border text-[#282828] min-w-[160px] overflow-auto text-[16px] font-[400] hidden largeTablet:block">
        <Today className="inline w-[18px] h-[18px] mt-[-2px] mr-[3px]" />
        {props.deadline.toDateString()}
      </th>
      <th className="border min-w-[136px]">
        <span
          className={twMerge(
            'p-[2px] pl-[10px] pr-[10px] rounded-xl border-1',
            statusClass.className
          )}
        >
          {statusClass.placeholder}
        </span>
      </th>
      <th className="border max-w-[100px] min-w-[56px] hidden mobile:block">
        <span
          className={twMerge(
            'p-[2px] pl-[10px] pr-[10px] rounded-xl border-1',
            priorityClass.className
          )}
        >
          {priorityClass.placeholder}
        </span>
      </th>
      <th className="border flex justify-end items-center h-[42px]">
        <More className="w-[24px] h-[24px] mr-[28px]" />
      </th>
    </tr>
  );
}
