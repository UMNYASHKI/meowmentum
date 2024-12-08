import Add from '@public/add.svg';
import { useState } from 'react';
import Clock from '@public/clock.svg';
import Calendar from '@public/calendar.svg';

interface AddTimeProps {
  taskId: number;
}

export function AddTime({ taskId }: AddTimeProps) {
  // const { isOpen, onOpen, onClose } = useDisclosure();
  const [isOpen, setIsOpen] = useState(false);

  const onOpen = () => setIsOpen(true);
  const onClose = () => setIsOpen(false);

  return (
    <div className="relative">
      <button className="flex items-center space-x-1" onClick={onOpen}>
        <span className="text-black">Add time</span>
        <Add className="dark:invert h-4 w-4" />
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 bg-white shadow-lg rounded-lg p-4 w-80 z-[100]">
          <div className="space-y-2">
            <div className="grid grid-cols-2 gap-4">
              <div className="flex flex-col">
                <div className="flex items-center space-x-2">
                  <Clock className="h-4 w-4" />
                  <span className="text-gray-500 text-sm font-medium">
                    Time
                  </span>
                </div>
                <input
                  type="text"
                  placeholder="0h"
                  className="border border-[#E5E5E5] bg-[#FAFAFA] rounded-lg mt-2 px-2 py-1 text-sm text-black"
                />
              </div>

              <div className="flex flex-col">
                <div className="flex items-center space-x-2">
                  <Calendar className="dark:invert h-4 w-4" />
                  <span className="text-gray-500 text-sm font-medium">
                    Date
                  </span>
                </div>
                <input
                  type="date"
                  className="border border-[#E5E5E5] bg-[#FAFAFA] rounded-lg mt-2 px-2 py-1 text-sm text-black"
                />
              </div>
            </div>

            <div className="flex justify-end">
              <button
                className="bg-[#676A6E] text-white text-sm px-4 py-2 rounded-lg hover:bg-gray-800"
                onClick={onClose}
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
