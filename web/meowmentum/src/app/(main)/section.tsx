'use client';

import Logo from '@public/logo.svg';
import Calendar from '@public/calendar.svg';
import CalendarActive from '@public/calendar-active.svg';
import Settings from '@public/settings.svg';
import Logout from '@public/logout.svg';
import Edit from '@public/edit-active.svg';
import EditActive from '@public/edit.svg';
import { useAuth } from '@providers/authProvider';
import { useLogOutMutation } from '@services/auth/authApi';
import { usePathname, useRouter } from 'next/navigation';

export default function Section() {
  const { logout } = useAuth();
  const [serverLogout] = useLogOutMutation();
  const path = usePathname();
  const router = useRouter();

  async function handleLogout() {
    await serverLogout({});
    logout();
  }

  return (
    <>
      <div className="hidden flex-col w-[20%] min-w-[240px] bg-[#282828] tablet:flex">
        <div className="flex flex-row m-[20px]">
          <Logo className="w-[20px] h-[20px] m-[10px] items-center" />
          <p className="font-[700] text-[20px] pt-1">Meowmentum</p>
        </div>
        <div className="flex flex-col h-full justify-between m-[30px]">
          <div>
            {path.includes('calendar') ? (
              <a
                href="../(auth)/login/page.tsx"
                className="flex flex-row rounded hover:bg-neutral-700 h-[47px] bg-white"
              >
                <CalendarActive className="w-[20px] h-[20px] m-[10px] items-center" />
                <p className="font-[500] text-[20px] text-black pt-[5px]">
                  Calendar
                </p>
              </a>
            ) : (
              <div
                onClick={() => router.push('/calendar')}
                className="flex flex-row rounded hover:bg-neutral-700 h-[47px]"
              >
                <Calendar className="w-[20px] h-[20px] m-[10px] items-center" />
                <p className="font-[500] text-[20px] pt-[5px]">Calendar</p>
              </div>
            )}

            {path.includes('tasks') || path === '/' ? (
              <div className="flex flex-row mt-[20px] bg-white rounded h-[47px]">
                <Edit className="w-[20px] h-[20px] m-[10px] items-center" />
                <p className="font-[500] text-[20px] text-black pt-[5px]">
                  Task List
                </p>
              </div>
            ) : (
              <div
                onClick={() => router.push('/tasks')}
                className="flex flex-row mt-[20px] bg-none rounded h-[47px]"
              >
                <EditActive className="w-[20px] h-[20px] m-[10px] items-center" />
                <p className="font-[500] text-[20px] text-white pt-[5px]">
                  Task List
                </p>
              </div>
            )}
          </div>
          <div>
            <div className="flex flex-row mt-[20px] rounded hover:bg-neutral-700 h-[47px]">
              <Settings className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[20px] pt-[5px]">Settings</p>
            </div>
            <div className="flex flex-row mt-[20px] mb-[20px] rounded hover:bg-neutral-700 h-[47px]">
              <Logout className="w-[20px] h-[20px] m-[10px] items-center" />
              <p
                onClick={handleLogout}
                className="font-[500] text-[20px] pt-[5px]"
              >
                Log Out
              </p>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
