'use client';

import React, { useState } from 'react';
import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
} from '@nextui-org/react';
import { TaskPriority } from '@/common/tasks';
import Calendar from '@public/calendar.svg';
import Add from '@public/add.svg';
import Priority from '@public/priority.svg';

type PageMode = 'create' | 'edit';

interface EditComponentProps {
  mode: PageMode;
  onClose: () => void;
}

const buttonStyle =
  'flex items-center justify-center p-2 px-4 h-8 rounded-lg text-[#FFFFF] bg-[#676A6E] hover:bg-[#BFC0C0]';

const EditComponent: React.FC<EditComponentProps> = ({ mode, onClose }) => {
  const [taskName, setTaskName] = useState('');
  const [description, setDescription] = useState('');
  const [deadline, setDeadline] = useState<Date | undefined>(undefined);
  const [priority, setPriority] = useState<TaskPriority | undefined>(undefined);

  const handleSave = () => {
    const payload = {
      name: taskName,
      description,
      deadline,
      priority,
    };

    console.log(
      `${mode === 'create' ? 'Create' : 'Update'} task payload:`,
      payload
    );
    onClose();
  };

  return (
    <Modal
      isOpen
      onClose={onClose}
      placement="center"
      backdrop="opaque"
      className="bg-[#FAFAFA] max-w-4xl w-full rounded-xl"
    >
      <ModalContent>
        {(onModalClose) => (
          <div className="p-6">
            <ModalHeader className="pb-4">
              <h2 className="text-xl font-semibold text-gray-800">
                {mode === 'create' ? 'Create Task' : 'Edit Task'}
              </h2>
            </ModalHeader>

            <ModalBody className="flex flex-col space-y-4">
              <div className="flex flex-col">
                <input
                  type="text"
                  value={taskName}
                  onChange={(e) => setTaskName(e.target.value)}
                  className="w-full px-4 py-3 border rounded-lg bg-white text-gray-700 placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                  placeholder="Task Name"
                />
              </div>

              <div className="flex flex-col">
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  className="w-full px-4 py-3 border rounded-lg bg-white text-gray-700 placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                  placeholder="Add your description."
                  rows={4}
                />
              </div>

              <div className="flex flex-row space-x-4 space-y-0">
                <button
                  onClick={() => console.log('Set Deadline')}
                  className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]"
                >
                  <Calendar className="text-white bg-transparent h-5 w-5" />
                  <span>Set deadline</span>
                </button>
                <button
                  onClick={() => console.log('Set Priority')}
                  className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-white bg-[#676A6E] hover:bg-[#BFC0C0]"
                >
                  <Priority className="h-6 w-6" />
                  <span>Set priority</span>
                </button>
                <button
                  onClick={() => console.log('Add Tags')}
                  className="flex items-center space-x-2 py-2 px-4 h-10 rounded-lg text-black bg-[#E5E5E5] hover:bg-[#BFC0C0]"
                >
                  <span>Add tags</span>
                  <Add className="dark:invert h-4 w-4"/>
                </button>
              </div>
            </ModalBody>

            <ModalFooter className="pt-4 space-x-4">
              <Button
                color="primary"
                variant="light"
                onPress={onModalClose}
                className="rounded-lg"
              >
                Cancel
              </Button>
              <Button
                color="primary"
                onPress={handleSave}
                className="rounded-lg"
              >
                {mode === 'create' ? 'Create Task' : 'Save Changes'}
              </Button>
            </ModalFooter>
          </div>
        )}
      </ModalContent>
    </Modal>
  );
};

export default EditComponent;
