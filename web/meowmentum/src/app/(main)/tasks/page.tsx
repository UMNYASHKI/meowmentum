'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import Add from '@public/add.svg';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';
import {
  TaskPriority,
  TaskResponse,
  TaskStatus,
} from '@services/tasks/tasksDtos';
import { useState } from 'react';
import { TagResponse } from '@services/tags/tagsDtos';
import TaskShortView from '@/components/tasks/taskShortView';
import { useDisclosure } from '@nextui-org/react';
import EditComponent from '@/components/tasks/edit/edit';

export default function Tasks() {
  const { isOpen, onOpen, onOpenChange } = useDisclosure();

  const [tags, setTags] = useState<TagResponse[]>([]);
  const [tasks, setTasks] = useState<TaskResponse[]>([]);

  function handleChangeFilter() {}

  return (
    <>
      <DashboardContainer>
        <DashboardHeader title="Task List" />
        <DashboardBody>
          <div className="mt-[40px] ml-[40px]">
            <p>
              <span className="text-black text-[28px] font-[600]">
                {tasks.length} Tasks{' '}
              </span>
              <span className="text-[#676A6E] text-[18px] font-[500]">
                / 18 total
              </span>
            </p>
            <br />
            <div>
              <FilterSelect
                className="mr-[20px]"
                name="Priority"
                placeholder="Priority"
                values={Object.values(TaskPriority).map((x) => x.toString())}
                onChange={handleChangeFilter}
              />
              <FilterSelect
                className="mr-[20px]"
                name="Tag"
                placeholder="Tag"
                values={tags.map((x) => x.name)}
                onChange={handleChangeFilter}
              />
              <FilterSelect
                name="Status"
                placeholder="Status"
                values={Object.values(TaskStatus).map((x) => x.toString())}
                onChange={handleChangeFilter}
              />
            </div>
          </div>
          <table className="mt-[36px] m w-full text-center">
            <tbody>
              <TaskShortView
                props={{
                  title: 'Task1Task1Task1Task1Task1Task1Task1Task1Task1Task1Task1Task1Task1',
                  deadline: new Date('2024-02-18'),
                  status: TaskStatus.Completed,
                  priority: TaskPriority.Medium,
                }}
              />
              <TaskShortView
                props={{
                  title: 'Task2',
                  deadline: new Date('2024-02-18'),
                  status: TaskStatus.Pending,
                  priority: TaskPriority.High,
                }}
              />
              <TaskShortView
                props={{
                  title: 'Task3',
                  deadline: new Date('2024-02-18'),
                  status: TaskStatus.Completed,
                  priority: TaskPriority.Low,
                }}
              />
              <TaskShortView
                props={{
                  title: 'Task4',
                  deadline: new Date('2024-02-18'),
                  status: TaskStatus.InProgress,
                  priority: TaskPriority.High,
                }}
              />
            </tbody>
          </table>

          <div className="absolute bottom-[50px] right-[50px] bg-black rounded-full hover:bg-neutral-700">
            <button
              onClick={onOpen} // Open the modal
              className="flex items-center justify-center w-[60px] h-[60px]"
            >
              <Add className="w-[30px] h-[30px]" />
            </button>
          </div>

          {isOpen && (
            <EditComponent
              mode="create"
              onClose={() => onOpenChange()}
              taskId={null}
            />
          )}
        </DashboardBody>
      </DashboardContainer>
    </>
  );
}
