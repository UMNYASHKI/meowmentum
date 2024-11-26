'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';

export default function Calendar() {
  return (
    <>
      <DashboardContainer>
        <DashboardHeader title={'Calendar'} />
        <DashboardBody></DashboardBody>
      </DashboardContainer>
    </>
  );
}