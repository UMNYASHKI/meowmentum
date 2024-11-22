'use client';

import { twMerge } from 'tailwind-merge';

interface FilterSelectProps {
  placeholder?: string;
  values: string[];
  onChange: (event: React.ChangeEvent<HTMLSelectElement>) => void;
  className?: string;
}

const FilterSelect: React.FC<FilterSelectProps> = ({
  placeholder,
  values,
  onChange,
  className = '',
}) => {
  return (
    <div>
      <select
        className={twMerge('', className)}
        onChange={onChange}
      >
        <option disabled selected>
          {placeholder}
        </option>
        {values.map((val) => (
          <option key={val}>{val}</option>
        ))}
      </select>
    </div>
  );
};

export default FilterSelect;
