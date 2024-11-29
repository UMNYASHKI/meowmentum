'use client';

import { twMerge } from 'tailwind-merge';
import NavigateNext from '../../../public/navigate_next.svg';

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
        <option key={val}>
          {val}
          <NavigateNext />
        </option>
      ))}
    </select>
  );
};

export default FilterSelect;
