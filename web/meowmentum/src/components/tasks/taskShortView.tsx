import { TaskPriority, TaskStatus, taskPriorities } from '@/common/tasks';
import More from '../../../public/more.svg';
import Edit from '../../../public/edit.svg';
import Delete from '../../../public/delete.svg';

import Today from '@public/today.svg';
import { twMerge } from 'tailwind-merge';
import EditComponent from '@components/tasks/edit/edit';
import { useDisclosure } from '@nextui-org/react';
import { SyntheticEvent } from 'react';
import {
  useDeleteTaskMutation,
  useLazyGetTaskQuery,
} from '@services/tasks/tasksApi';

interface TaskShortViewProps {
  id: number;
  title: string;
  deadline: Date | undefined;
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
    type: 'Pending',
    className: 'border-[#C9C9C9] bg-[#EBEBEB] text-[#959595]',
    placeholder: 'Not Started',
  },
  {
    type: 'InProgress',
    className: 'border-[#F8D255] bg-[#FFF8D4] text-[#E5AC09]',
    placeholder: 'In Progress',
  },
  {
    type: 'Completed',
    className: 'border-[#81D164] bg-[#EBFFDF] text-[#41B52D]',
    placeholder: 'Done',
  },
];
const priorityClasses: TypeClasses<TaskPriority>[] = [
  {
    type: taskPriorities[0],
    className: 'border-[#81D164] bg-[#EBFFDF] text-[#41B52D]',
    placeholder: 'P2',
  },
  {
    type: taskPriorities[1],
    className: 'border-[#F8D255] bg-[#FFF8D4] text-[#E5AC09]',
    placeholder: 'P1',
  },
  {
    type: taskPriorities[2],
    className: 'border-[#F2B8B8] bg-[#FFE8E8] text-[#DD6969]',
    placeholder: 'P0',
  },
];

export default function TaskShortView({
  props,
}: {
  props: TaskShortViewProps;
}) {
  const [deleteTask] = useDeleteTaskMutation();

  const { isOpen, onOpen, onOpenChange } = useDisclosure();
  async function handleDelete() {
    console.log(props.id);
    await deleteTask(props.id);
  }
  const statusClass: TypeClasses<TaskStatus> =
    statusClasses.find((x) => x.type === props.status) ?? statusClasses[0];
  const priorityClass: TypeClasses<TaskPriority> =
    priorityClasses.find((x) => x.type === props.priority) ??
    priorityClasses[0];
  return (
    <tr className="h-[42px]">
      <th className="border w-[42px]"> </th>
      <th className="text-left min-w-[140px] max-w-[220px] overflow-auto border pl-[12px] pr-[12px] text-black text-[20px] font-[400]">
        {props.title}
      </th>
      <th className="border text-[#282828] min-w-[140px] overflow-auto text-[16px] font-[400]">
        <Today className="inline w-[18px] h-[18px] mt-[-2px] mr-[3px]" />
        {props.deadline === undefined
          ? new Date('01-01-0000').toDateString()
          : props.deadline.toString()}
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
      <th className="border max-w-[100px] min-w-[56px] ">
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
        <Edit
          onClick={onOpen}
          className="w-[24px] h-[24px] mr-[28px] dark:invert"
        />
        <Delete
          onClick={handleDelete}
          className="w-[24px] h-[24px]  dark:invert"
        />
      </th>
      {isOpen && (
        <EditComponent
          mode="edit"
          onClose={() => onOpenChange()}
          taskId={props.id}
        />
      )}
    </tr>
  );
}
