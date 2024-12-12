'use client';

import React, { useState, useEffect } from 'react';
import {
  Dropdown,
  DropdownItem,
  DropdownMenu,
  DropdownTrigger,
} from '@nextui-org/dropdown';
import { CalendarDate, SharedSelection } from '@nextui-org/react';
import { Calendar as NextUiCalendar } from '@nextui-org/calendar';
import { parseDate } from '@internationalized/date';
import { format } from 'date-fns';
import Calendar from '@public/calendar.svg';
import Priority from '@public/priority.svg';
import Add from '@public/add.svg';
import { ITag } from '@/common/tags';
import { TaskPriority, TaskStatus } from '@/common/tasks';

interface ActionButtonsProps {
  deadline: Date | undefined;
  setDeadline: React.Dispatch<React.SetStateAction<Date | undefined>>;
  priority: string | undefined;
  setPriority: React.Dispatch<React.SetStateAction<string | undefined>>;
  tags: number[];
  setTags: React.Dispatch<React.SetStateAction<number[]>>;
  availableTags: ITag[]; // Tags fetched from API
  status: string | undefined;
  setStatus: React.Dispatch<React.SetStateAction<string | undefined>>;
}

// @ts-ignore
export default function ActionButtons({
  deadline,
  setDeadline,
  priority,
  setPriority,
  tags,
  setTags,
  availableTags,
  status,
  setStatus,
}: ActionButtonsProps) {
  const [showCalendar, setShowCalendar] = useState(false);
  const [selectedPriority, setSelectedPriority] = useState(
    new Set(['Set Priority'])
  );
  const [selectedTags, setSelectedTags] = useState(new Set(tags));
  const [selectedStatus, setSelectedStatus] = useState(new Set(['Set Status']));

  useEffect(() => {
    if (priority !== undefined) {
      setSelectedPriority(new Set([priority]));
    }

    if (status != undefined) {
      setSelectedStatus(new Set([status]));
    }

    setSelectedTags(new Set(tags));
  }, [priority, tags, status]);

  const selectedTagValue = React.useMemo(
    () => Array.from(selectedTags).join(', ').replaceAll('_', ' '),
    [selectedTags]
  );
  const handleDateChange = (date: CalendarDate) => {
    setDeadline(date.toDate(Intl.DateTimeFormat().resolvedOptions().timeZone));
    setShowCalendar(false);
  };

  const handlePriorityChange = (keys: SharedSelection) => {
    setSelectedPriority(keys as Set<TaskPriority>);
    // @ts-ignore
    setPriority(keys.currentKey as TaskPriority);
  };

  const handleStatusChange = (keys: SharedSelection) => {
    setSelectedStatus(keys as Set<TaskStatus>);
    // @ts-ignore
    setStatus(keys.currentKey as TaskStatus);
  };

  const handleTagsChange = (keys: SharedSelection) => {
    let selectedTags = keys as Set<number>;
    setSelectedTags(selectedTags);
    setTags(Array.from(selectedTags));
  };

  return (
    <div className="flex flex-row space-x-4 space-y-0">
      <button
        onClick={() => setShowCalendar(!showCalendar)}
        className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]"
      >
        <Calendar className="text-white bg-transparent h-5 w-5" />
        <span>
          {deadline ? format(deadline, 'yyyy-MM-dd') : 'Set deadline'}
        </span>
      </button>
      {showCalendar && (
        <div
          className="absolute top-11 left-0 z-10 rounded-lg shadow-lg bg-none"
          style={{ width: 'fit-content' }}
        >
          <NextUiCalendar
            aria-label="Date (Uncontrolled)"
            defaultValue={parseDate(format(new Date(), 'yyyy-MM-dd'))}
            minValue={parseDate(format(new Date(), 'yyyy-MM-dd'))}
            onChange={handleDateChange}
          />
        </div>
      )}

      <Dropdown>
        <DropdownTrigger>
          <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]">
            <Priority className="h-6 w-6" />
            <span className="capitalize">{selectedPriority} </span>
          </button>
        </DropdownTrigger>
        <DropdownMenu
          aria-label="Set Priority"
          variant="flat"
          disallowEmptySelection
          selectionMode="single"
          selectedKeys={selectedPriority}
          onSelectionChange={handlePriorityChange}
        >
          <DropdownItem key="Low">Low</DropdownItem>
          <DropdownItem key="Medium">Medium</DropdownItem>
          <DropdownItem key="High">High</DropdownItem>
        </DropdownMenu>
      </Dropdown>

      <Dropdown>
        <DropdownTrigger>
          <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]">
            <Priority className="h-6 w-6" />
            <span className="capitalize">{selectedStatus} </span>
          </button>
        </DropdownTrigger>
        <DropdownMenu
          aria-label="Set Status"
          variant="flat"
          disallowEmptySelection
          selectionMode="single"
          selectedKeys={selectedStatus}
          onSelectionChange={handleStatusChange}
        >
          <DropdownItem key="Pending">Pending</DropdownItem>
          <DropdownItem key="InProgress">In Progress</DropdownItem>
          <DropdownItem key="Completed">Completed</DropdownItem>
        </DropdownMenu>
      </Dropdown>

      <Dropdown>
        <DropdownTrigger>
          <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-black bg-[#E5E5E5] hover:bg-[#BFC0C0]">
            <span>Add tags</span>
            <Add className="dark:invert h-4 w-4" />
          </button>
        </DropdownTrigger>
        <DropdownMenu
          aria-label="Select Tags"
          variant="flat"
          closeOnSelect={false}
          disallowEmptySelection
          selectionMode="multiple"
          selectedKeys={selectedTagValue}
          onSelectionChange={handleTagsChange}
        >
          {availableTags.map((tag) => (
            <DropdownItem key={tag.id}>{tag.name}</DropdownItem>
          ))}
        </DropdownMenu>
      </Dropdown>
    </div>
  );
}
