'use client';

import StatisticsHeader from '@components/statistics/statisticsHeader';
import StatisticsBody from '@components/statistics/statisticsBody';
import {
  DownloadExcelRequest,
  useDownloadExcel,
} from '@utils/useDownloadExcel';
import { useState } from 'react';

export default function StatisticsView() {
  const { downloadExcelReport, isLoading, error } = useDownloadExcel();
  const [iframeUrl, setIframeUrl] = useState<string | null>(null);

  const handleDownloadReport = async (requestData: DownloadExcelRequest) => {
    const iframeUrl = await downloadExcelReport(requestData);
    if (iframeUrl) {
      setIframeUrl(iframeUrl);
    }
  };

  return (
    <div className="ml-5 mt-2">
      <StatisticsHeader downloadExcel={handleDownloadReport} />
      <StatisticsBody
        isLoading={isLoading}
        error={error}
        iframeLink={iframeUrl}
      />
    </div>
  );
}
