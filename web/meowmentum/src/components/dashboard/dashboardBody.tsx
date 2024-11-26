export default function DashboardBody({
  children,
}: {
  children?: React.ReactNode;
}) {
  return (
    <div className="relative flex flex-col bg-[#FFFFFF] m-[25px] h-full mt-[0] rounded-xl border-1 border-[#E5E5E5]">
      {children}
    </div>
  );
}
