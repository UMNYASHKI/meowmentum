import { ReactNode } from 'react';

export default function DashboardContainer({
  children,
}: {
  children: ReactNode;
}) {
  return <div className="flex flex-col w-[100%] bg-[#FAFAFA]">{children}</div>;
}
