'use client';

import { useState } from 'react';
import Menu from '@public/menu.svg';
import Avatar from '@public/avatar.svg';
import Close from '@public/close.svg';
import Logo from '@public/logo.svg';
import Calendar from '@public/calendar.svg';
import Edit from '@public/edit.svg';
import Settings from '@public/settings.svg';
import Logout from '@public/logout.svg';
import { useAuth } from '@providers/authProvider';
import { useLogOutMutation } from '@services/auth/authApi';
import { useRouter } from 'next/navigation';

interface DashboardHeaderProps {
  title: string;
}

export default function DashboardHeader({ title }: DashboardHeaderProps) {
  const [isMenuOpened, setIsMenuOpened] = useState<boolean>(false);
  const { logout } = useAuth();
  const [serverLogout] = useLogOutMutation();
  const router = useRouter();

  async function handleLogout() {
    await serverLogout({});
    logout();
  }

  return (
    <>
      <div className="flex justify-between bg-[#FFFFFF] m-[25px] h-[64px] rounded-xl border-1 border-[#E5E5E5]">
        <div className="flex ml-[30px] items-center">
          <h1 className="text-[28px] font-[600] text-[#000000]">{title}</h1>
        </div>
        <div className="flex flex-row items-center">
          <div className="mr-[15px] tablet:hidden">
            <Menu onClick={() => setIsMenuOpened(true)} className="w-8 h-8" />
          </div>
          <div className="border-l-2 border-gray-300 h-[48px] mt-2 mb-2"></div>
          <div className="mr-[20px] ml-[15px]">
            <Avatar className="w-8 h-8" />
          </div>
        </div>
      </div>
      {isMenuOpened ? (
        <div className="absolute top-0 left-0 w-full h-full bg-black z-10 tablet:hidden">
          <div className="absolute top-[32px] right-[25px] hover:bg-neutral-700 rounded-3xl">
            <Close
              onClick={() => setIsMenuOpened(false)}
              className="w-[40px] h-[40px] bg-white"
            />
          </div>
          <div className="flex flex-col items-center pl-[30px] pr-[30px]">
            <div className="flex flex-row mt-[30px] w-full">
              <Logo className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[28px]">Meowmentum</p>
            </div>
            <div
              onClick={() => router.push('/calendar')}
              className="flex flex-row mt-[30px] w-full justify-center pt-[15px] pb-[15px] rounded-xl hover:bg-neutral-700"
            >
              <Calendar className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[28px]">Calendar</p>
            </div>
            <div
              onClick={() => router.push('/tasks')}
              className="flex flex-row mt-[20px] w-full justify-center pt-[15px] pb-[15px] rounded-xl hover:bg-neutral-700"
            >
              <Edit className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[28px]">Task List</p>
            </div>
            <div className="flex flex-row mt-[20px] w-full justify-center pt-[15px] pb-[15px] rounded-xl hover:bg-neutral-700">
              <Settings className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[28px]">Settings</p>
            </div>
            <div
              onClick={handleLogout}
              className="flex flex-row mt-[20px] w-full justify-center pt-[15px] pb-[15px] rounded-xl hover:bg-neutral-700"
            >
              <Logout className="w-[20px] h-[20px] m-[10px] items-center" />
              <p className="font-[500] text-[28px]">Log Out</p>
            </div>
          </div>
        </div>
      ) : (
        ''
      )}
    </>
  );
}
