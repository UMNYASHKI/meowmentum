'use client';

import FilterSelect from '@common/filterSelect';

export default function TasksBody() {
  return (
    <>
      <div className="flex flex-col bg-[#FFFFFF] m-[25px] h-full mt-[0] rounded-xl border-1 border-[#E5E5E5]">
        <div>
          <p>4/18 tasks total</p>
        </div>
        <div>
          <FilterSelect
            placeholder="Tag"
            values={['first', 'second', 'third']}
            onChange={(e)=>console.log(e.target.value)}
          />
        </div>
      </div>
    </>
  );
}
