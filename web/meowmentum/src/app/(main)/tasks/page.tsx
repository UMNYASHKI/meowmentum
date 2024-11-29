'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import Add from '@public/add.svg';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';
import { useDisclosure } from '@nextui-org/react';
import EditComponent from '@/components/tasks/edit/edit';

export default function Tasks() {
  const { isOpen, onOpen, onOpenChange } = useDisclosure();

  return (
    <>
      <DashboardContainer>
        <DashboardHeader title="Task List" />
        <DashboardBody>
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
