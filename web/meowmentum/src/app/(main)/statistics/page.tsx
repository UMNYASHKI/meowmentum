import DashboardContainer from '@components/dashboard/dashboardContainer';
import DashboardHeader from '@components/dashboard/dashboardHeader';
import DashboardBody from '@components/dashboard/dashboardBody';
import StatisticsView from '@components/statistics/statisticsView';

export default function Statistics() {
  return (
    <>
      <DashboardContainer>
        <DashboardHeader title={'Statistics'} />
        <DashboardBody>
          <StatisticsView></StatisticsView>
        </DashboardBody>
      </DashboardContainer>
    </>
  );
}
