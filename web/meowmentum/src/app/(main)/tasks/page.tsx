import TasksHeader from '@/app/(main)/tasks/header';
import TasksBody from '@/app/(main)/tasks/body';

export default function Tasks() {
  return (
    <>
      <div className="flex flex-col w-[100%]">
        <TasksHeader />
        <TasksBody />
      </div>
    </>
  );
}
