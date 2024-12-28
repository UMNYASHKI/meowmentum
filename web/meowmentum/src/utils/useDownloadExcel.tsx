import { useState } from 'react';
import { ReportType, StatisticsDateFormat } from '@/common/statistics';
import { downloadBlobAsFile } from '@utils/helpers';
import { format } from 'date-fns';

export interface DownloadExcelRequest {
  type: ReportType;
  from: Date | null;
  to: Date | null;
}

const reportsRoute: string =
  process.env.NODE_ENV !== 'production' ? 'api/report' : 'core/api/report';

const ReportsEndpointsMap: Record<ReportType, string> = {
  completedTasks: 'completed-tasks',
  deadlineReport: 'deadline-report',
  tagReport: 'tag-report',
};

const getReportGenerationRoute = (reportType: ReportType) => {
  const endpoint = ReportsEndpointsMap[reportType];
  return `${process.env.NEXT_PUBLIC_API_ENDPOINT}/${reportsRoute}/${endpoint}`;
};

export const useDownloadExcel = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const downloadExcelReport = async ({
    type,
    from,
    to,
  }: DownloadExcelRequest) => {
    setIsLoading(true);
    setError(null);

    const token = localStorage.getItem('token');
    const url =
      getReportGenerationRoute(type) +
      `?startDate=${from?.toISOString()}&endDate=${to?.toISOString()}`;
    try {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
          Accept:
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        },
      });

      if (!response.ok) {
        throw new Error('Failed to download report');
      }

      const blob = await response.blob();
      const filename = `${type}-${from ? format(from, StatisticsDateFormat) : 'unknown'}-${to ? format(to, StatisticsDateFormat) : 'unknown'}.xlsx`;
      downloadBlobAsFile(blob, filename);

      return URL.createObjectURL(blob);
    } catch (err) {
      setError('Error downloading the report');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  return { downloadExcelReport, isLoading, error };
};
