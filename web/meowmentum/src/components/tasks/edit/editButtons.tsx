'use client';

import React, { useState, useMemo } from 'react';
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
import { TaskPriority, TaskPriorityMapping } from '@/common/tasks';

interface ActionButtonsProps {
  deadline: Date | undefined;
  setDeadline: React.Dispatch<React.SetStateAction<Date | undefined>>;
  priority: number | undefined;
  setPriority: React.Dispatch<React.SetStateAction<number | undefined>>;
  tags: number[];
  setTags: React.Dispatch<React.SetStateAction<number[]>>;
  availableTags: ITag[]; // Tags fetched from API
}

export default function ActionButtons({
  deadline,
  setDeadline,
  priority,
  setPriority,
  tags,
  setTags,
  availableTags,
}: ActionButtonsProps) {
  const [showCalendar, setShowCalendar] = useState(false);
  const [selectedPriority, setSelectedPriority] = useState(
    new Set(priority ? [TaskPriorityMapping[priority]] : ['Set priority'])
  );
  console.log(selectedPriority);
  const [selectedTags, setSelectedTags] = useState(new Set<number>());

  const selectedPriorityValue = useMemo(
    () => Array.from(selectedPriority).join(', ').replaceAll('_', ' '),
    [selectedPriority]
  );

  const tagsDict = availableTags.reduce(
    (dict, tag) => {
      dict[tag.id] = tag;
      return dict;
    },
    {} as Record<number, ITag>
  );

  const selectedTagValue = useMemo(() => {
    // Map selected IDs to their names from tagsDict
    const selectedNames = Array.from(selectedTags)
      .map((id) => tagsDict[id]?.name || '')
      .filter((name) => name !== null && name !== '');
    return selectedNames.join(', '); // Join names and replace underscores if needed
  }, [selectedTags, tagsDict]);

  const handleDateChange = (date: CalendarDate) => {
    setDeadline(date.toDate(Intl.DateTimeFormat().resolvedOptions().timeZone));
    setShowCalendar(false);
  };

  const handlePriorityChange = (keys: SharedSelection) => {
    setSelectedPriority(keys as Set<TaskPriority>);
    // @ts-ignore
    setPriority(TaskPriorityMapping[keys.currentKey as TaskPriority]);
  };

  const handleTagsChange = (keys: SharedSelection) => {
    console.log(keys);
    setSelectedTags(keys as Set<number>);
    setTags(Array.from(keys as Set<number>));
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
            onChange={handleDateChange}
          />
        </div>
      )}

      <Dropdown>
        <DropdownTrigger>
          <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]">
            <Priority className="h-6 w-6" />
            <span className="capitalize">{selectedPriorityValue}</span>
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
          <button className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-black bg-[#E5E5E5] hover:bg-[#BFC0C0]">
            <span>
              {selectedTagValue == null || selectedTagValue == ''
                ? 'Add tags'
                : ''}
            </span>
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
