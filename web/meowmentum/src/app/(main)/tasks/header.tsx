export default function TasksHeader() {
  return (
    <>
      <div className="flex justify-between bg-[#FFFFFF] m-[25px] h-[64px] rounded-xl border-1 border-[#E5E5E5]">
        <div className="flex ml-[20px] items-center">
          <h1 className="text-[28px] font-[600] text-[#000000]">Task List</h1>
        </div>
        <div className="flex flex-row items-center">
          <div className="border-l-2 border-gray-300 h-[48px] mt-2 mb-2"></div>
          <div className="mr-[20px] ml-[15px]">Logo</div>
        </div>
      </div>
    </>
  );
}
