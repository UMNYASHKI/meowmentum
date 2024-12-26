'use client';

import DashboardHeader from '@/components/dashboard/dashboardHeader';
import DashboardBody from '@/components/dashboard/dashboardBody';
import DashboardContainer from '@/components/dashboard/dashboardContainer';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

export default function Calendar() {
  return (
    <>
      <DashboardContainer>
        <DashboardHeader title={'Calendar'} />

        <DashboardBody>
          <FullCalendar
            plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
            initialView="timeGridWeek"
            headerToolbar={{
              left: 'prev,next today',
              center: 'title',
              right: 'dayGridMonth,timeGridWeek,timeGridDay',
            }}
            events={[]}
            height="100%"
          />
        </DashboardBody>
      </DashboardContainer>
    </>
  );
}
