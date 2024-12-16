'use client';

import { twMerge } from 'tailwind-merge';
import NavigateNext from '../../../public/navigate_next.svg';
import { Fragment } from 'react';

interface FilterSelectProps {
  placeholder?: string;
  name: string;
  values: string[];
  onChange: (event: React.ChangeEvent<HTMLSelectElement>) => void;
  className?: string;
}

const FilterSelect: React.FC<FilterSelectProps> = ({
  placeholder,
  name,
  values,
  onChange,
  className = '',
}) => {
  return (
    <select
      className={twMerge(
        'p-[8px] bg-white rounded-2xl border-1 border-[#E0E0E0] text-[18px] text-[#676A6E]',
        className
      )}
      name={name}
      onChange={onChange}
    >
      <option className="hidden" disabled selected>
        {placeholder}
      </option>
      {values.map((val) => (
        <Fragment key={val}>
          <option key={val}>{val}</option>
          <NavigateNext />
        </Fragment>
      ))}
    </select>
  );
};

export default FilterSelect;
