import { ModalBody } from '@nextui-org/modal';
import TimeLogsHeader from '@components/time-logs/timeLogsHeader';
import TimeLogsBody from '@components/time-logs/timeLogsBody';


export default function TimeLogs() {
  return (
    <ModalBody className="mt-4">
      <TimeLogsHeader />
      <TimeLogsBody />
    </ModalBody>
  );
}