'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import FilterSelect from '@common/filterSelect';
import Add from '@public/add.svg';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';

export default function Tasks() {
  return (
    <>
      <DashboardContainer>
        <DashboardHeader title={'Task List'} />
        <DashboardBody>
          <div className="absolute bottom-[50px] right-[50px] bg-black rounded-full hover:bg-neutral-700">
            <Add className="w-[30px] h-[30px] m-[15px]" />
          </div>
        </DashboardBody>
      </DashboardContainer>
    </>
  );
}
