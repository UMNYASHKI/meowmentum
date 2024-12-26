'use client';

import {
  Dropdown,
  DropdownItem,
  DropdownMenu,
  DropdownTrigger,
} from '@nextui-org/dropdown';
import Priority from '@public/priority.svg';
import React, { useState } from 'react';
import { Button, CalendarDate, SharedSelection } from '@nextui-org/react';
import {
  StatisticsDateFormat,
  ReportType,
  ReportViewNameByType,
} from '@/common/statistics';
import Calendar from '@public/calendar.svg';
import { format } from 'date-fns';
import { Calendar as NextUiCalendar } from '@nextui-org/calendar';
import { parseDate } from '@internationalized/date';
import { DownloadExcelRequest } from '@utils/useDownloadExcel';

interface StatisticsHeaderProps {
  downloadExcel: (request: DownloadExcelRequest) => Promise<void>;
}

export default function StatisticsHeader({
  downloadExcel,
}: StatisticsHeaderProps) {
  const [selectedReport, setSelectedReport] = useState<Set<ReportType>>(
    new Set<ReportType>(['completedTasks'])
  );
  const [showFromCalendar, setShowFromCalendar] = useState(false);
  const [showToCalendar, setShowToCalendar] = useState(false);
  const [fromDate, setFromDate] = useState<Date | null>(null);
  const [toDate, setToDate] = useState<Date | null>(null);

  const handleReportChange = (keys: SharedSelection) => {
    setSelectedReport(keys as Set<ReportType>);
  };

  const handleFromDateChange = (date: CalendarDate) => {
    setFromDate(new Date(date.toString()));
    setShowFromCalendar(false);
  };

  const handleToDateChange = (date: CalendarDate) => {
    setToDate(new Date(date.toString()));
    setShowToCalendar(false);
  };

  const reportType = selectedReport.values().next().value;
  const reportName =
    reportType !== undefined
      ? ReportViewNameByType[reportType]
      : 'Unknown type';

  const formattedFromDate = fromDate
    ? format(fromDate, StatisticsDateFormat)
    : 'Set from';
  const formattedToDate = toDate
    ? format(toDate, StatisticsDateFormat)
    : 'Set to';

  const handleGenerateReport = () => {
    if (fromDate && toDate && reportType) {
      const requestData: DownloadExcelRequest = {
        type: reportType,
        from: fromDate,
        to: toDate,
      };

      downloadExcel(requestData);
      console.log('Generating report for:', { reportName, fromDate, toDate });
    } else {
      console.error('Both from date and to date are required!');
    }
  };

  return (
    <div className="flex flex-col md:flex-row md:items-center md:space-x-4 space-y-4 md:space-y-0 w-full m-5">
      <div>
        <Dropdown>
          <DropdownTrigger>
            <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]">
              <Priority className="h-6 w-6" />
              <span className="capitalize">{reportName}</span>
            </button>
          </DropdownTrigger>
          <DropdownMenu
            aria-label="Set Report Type"
            variant="flat"
            disallowEmptySelection
            selectionMode="single"
            selectedKeys={selectedReport}
            onSelectionChange={handleReportChange}
          >
            <DropdownItem key="completedTasks">Completed Tasks</DropdownItem>
            <DropdownItem key="tagReport">Task Tags</DropdownItem>
            <DropdownItem key="deadlineReport">Deadline Report</DropdownItem>
          </DropdownMenu>
        </Dropdown>
      </div>

      <div className="flex flex-col space-y-4 md:flex-row md:space-x-4 md:space-y-0 md:w-auto">
        <div className="relative sm:w-auto">
          <button
            onClick={() => setShowFromCalendar(!showFromCalendar)}
            className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0] sm:w-auto"
          >
            <Calendar className="text-white bg-transparent h-5 w-5" />
            <span>{formattedFromDate}</span>
          </button>
          {showFromCalendar && (
            <div
              className="absolute top-full left-0 z-10 rounded-lg shadow-lg bg-none mt-2"
              style={{ width: 'fit-content' }}
            >
              <NextUiCalendar
                aria-label="Date (Uncontrolled)"
                defaultValue={parseDate(
                  format(new Date(), StatisticsDateFormat)
                )}
                onChange={handleFromDateChange}
              />
            </div>
          )}
        </div>

        <div className="relative sm:w-auto">
          <button
            onClick={() => setShowToCalendar(!showToCalendar)}
            className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0] sm:w-auto"
          >
            <Calendar className="text-white bg-transparent h-5 w-5" />
            <span>{formattedToDate}</span>
          </button>
          {showToCalendar && (
            <div
              className="absolute top-full left-0 z-10 rounded-lg shadow-lg bg-none mt-2"
              style={{ width: 'fit-content' }}
            >
              <NextUiCalendar
                aria-label="Date (Uncontrolled)"
                defaultValue={parseDate(
                  format(new Date(), StatisticsDateFormat)
                )}
                onChange={handleToDateChange}
              />
            </div>
          )}
        </div>
      </div>

      <div className="mt-4 sm:mt-0 w-full md:w-auto md:ml-auto">
        <Button
          variant="light"
          onPress={handleGenerateReport}
          className="rounded-lg font-medium sm:w-auto bg-[#282828]"
        >
          Generate Report
        </Button>
      </div>
    </div>
  );
}
