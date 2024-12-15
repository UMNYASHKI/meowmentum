'use client';

import React, { useEffect, useState } from 'react';
import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
} from '@nextui-org/react';
import { Divider } from '@nextui-org/divider';
import ActionButtons from '@/components/tasks/edit/editButtons';
import { ITag } from '@/common/tags';
import { useLazyGetAllTagsQuery } from '@services/tags/tagApi';
import { CreateTaskRequest, TaskResponse } from '@services/tasks/tasksDtos';
import {
  useCreateTaskMutation,
  useLazyGetTaskQuery,
} from '@services/tasks/tasksApi';
import { setPopupMessage } from '@/lib/slices/app/appSlice';
import { useAppDispatch } from '@/lib/hooks';
import {
  ReverseTaskPriorityMapping,
  ReverseTaskStatusMapping,
  TaskPriority,
  TaskPriorityMapping,
  TaskStatus,
  TaskStatusMapping,
} from '@/common/tasks';

type PageMode = 'create' | 'edit';

interface EditComponentProps {
  mode: PageMode;
  onClose: () => void;
  taskId: number | null;
}

export default function EditComponent({
  mode,
  onClose,
  taskId,
}: EditComponentProps) {
  const dispatch = useAppDispatch();
  const [triggerGetAllTags] = useLazyGetAllTagsQuery({});
  const [createTask] = useCreateTaskMutation();
  const [triggerGetTask] = useLazyGetTaskQuery();
  const [taskName, setTaskName] = useState<string | undefined>(undefined);
  const [description, setDescription] = useState<string | undefined>(undefined);
  const [deadline, setDeadline] = useState<Date | undefined>(undefined);
  const [priority, setPriority] = useState<string | undefined>(undefined);
  const [status, setStatus] = useState<string | undefined>(undefined);
  const [tags, setTags] = useState<number[]>([]); // Tags that are in task + Selected tags
  const [availableTags, setAvailableTags] = useState<ITag[]>([]); // Tags fetched from API

  useEffect(() => {
    triggerGetAllTags()
      .unwrap()
      .then((tags) => setAvailableTags(tags));
  }, [triggerGetAllTags]);

  useEffect(() => {
    if (taskId) {
      triggerGetTask({
        taskId,
        status: [],
        tagIds: [],
        priorities: [],
      })
        .unwrap()
        .then((tasks: TaskResponse[]) => {
          if (tasks.length < 0) {
            setError();
            return;
          }
          const task = tasks[0];

          setTaskName(task.title);
          setDescription(task.description);
          setDeadline(task.deadline);
          setPriority(
            task.priority != undefined
              ? TaskPriorityMapping[task.priority]
              : undefined
          );
          setStatus(
            task.status != undefined
              ? ReverseTaskStatusMapping[task.status]
              : undefined
          ),
            setTags(task.tags.map((t) => t.id));
        })
        .catch((error) => {
          console.error('Error fetching task:', error);
        });
    }
  }, [taskId]);

  const setError = () => {
    dispatch(
      setPopupMessage({
        message: `Failed to create task`,
        type: 'error',
        isVisible: true,
      })
    );
  };

  const handleSave = async () => {
    const actualTags = tags.filter(
      (item) => !isNaN(Number(item)) && item.toString() !== ' '
    );
    const payload: CreateTaskRequest = {
      id: taskId,
      title: taskName ?? '',
      description: description ?? '',
      deadline: deadline,
      priority:
        priority != undefined
          ? ReverseTaskPriorityMapping[priority as TaskPriority]
          : undefined,
      status:
        status != undefined
          ? TaskStatusMapping[status as TaskStatus]
          : undefined,
      tagIds: actualTags,
    };

    try {
      const response = await createTask(payload);

      if (response?.data) {
        onClose();
        return;
      } else {
        setError();
      }
    } catch (error) {
      setError();
    }
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
                  placeholder={taskName ?? 'Task Name'}
                />
              </div>

              <div className="flex flex-col">
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  className="w-full px-4 py-3 border rounded-lg bg-white text-gray-700 placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                  placeholder={description ?? 'Add your description.'}
                  rows={4}
                />
              </div>

              <ActionButtons
                deadline={deadline}
                setDeadline={setDeadline}
                priority={priority}
                setPriority={setPriority}
                tags={tags}
                setTags={setTags}
                availableTags={availableTags}
                status={status}
                setStatus={setStatus}
              />
            </ModalBody>

            <Divider className="my-4 bg-[#E5E5E5]" />
            <ModalBody className="h-1/2">{'Time logs'}</ModalBody>
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
                Save
              </Button>
            </ModalFooter>
          </div>
        )}
      </ModalContent>
    </Modal>
  );
}
