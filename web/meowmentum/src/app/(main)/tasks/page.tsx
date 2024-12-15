'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import Add from '@public/add.svg';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';
import { TaskResponse } from '@services/tasks/tasksDtos';
import { useEffect, useState } from 'react';
import { TagResponse } from '@services/tags/tagsDtos';
import TaskShortView from '@/components/tasks/taskShortView';
import { useDisclosure } from '@nextui-org/react';
import EditComponent from '@/components/tasks/edit/edit';
import FilterSelect from '@common/filterSelect';
import {
  taskPriorities,
  TaskPriority,
  TaskPriorityMapping,
  TaskStatus,
} from '@/common/tasks';
import { useLazyGetTaskQuery } from '@services/tasks/tasksApi';
import { ReactDOM } from 'next/dist/server/future/route-modules/app-page/vendored/rsc/entrypoints';
import { useLazyGetAllTagsQuery } from '@services/tags/tagApi';

interface FiltersModel
{

}

export default function Tasks() {
  const { isOpen, onOpen, onOpenChange } = useDisclosure();
  const [getTasks] = useLazyGetTaskQuery();
  const [getTags] = useLazyGetAllTagsQuery();
  const [tags, setTags] = useState<TagResponse[]>([]);
  const [tasks, setTasks] = useState<TaskResponse[]>([]);

  useEffect(() => {
    getTasks({ priorities: [], status: [], tagIds: [], taskId: undefined })
      .unwrap()
      .then((data) => {
        setTasks(data);
        let processed = tasks;
        for (let i = 0; i < data.length; i++) {
          processed[i] = {
            ...data[i],
            priority: getPriority(data[i].priority as any),
            status: getStatus(data[i].status as any),
          };
        }
        setTasks(processed);
        console.log(processed);
      });
  }, [getTasks, tasks]);

  function handleChangeFilter() {}

  function getPriority(priority: number): TaskPriority {
    switch (priority) {
      case 2:
        return 'Medium';
      case 3:
        return 'High';
      case 1:
        return 'Low';
    }
  }

  function getStatus(status: number): TaskStatus {
    switch (status) {
      case 2:
        return 'InProgress';
      case 3:
        return 'Completed';
      case 1:
        return 'Pending';
    }
  }

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
                values={taskPriorities.map((x) => x.toString())}
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
                values={['Pending', 'InProgress','Completed']}
                onChange={handleChangeFilter}
              />
            </div>
          </div>
          <table className="mt-[36px] m w-full text-center">
            <tbody>
              {tasks.map((x) => {
                return (
                  <TaskShortView
                    key={x.id}
                    props={{
                      title: x.title ?? '',
                      deadline: x.deadline ?? new Date('01-01-0000'),
                      status: x.status ?? 'Completed',
                      priority: x.priority ?? 'High',
                    }}
                  />
                );
              })}
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
