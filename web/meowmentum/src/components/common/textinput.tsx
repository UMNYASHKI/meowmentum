import { twMerge } from 'tailwind-merge';

interface TextInputProps {
  label: string;
  type?: string;
  name: string;
  placeholder?: string;
  value: string;
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
  disabled?: boolean;
  className?: string;
}

const TextInput: React.FC<TextInputProps> = ({
  label,
  type = 'text',
  name,
  placeholder,
  value,
  onChange,
  disabled = false,
  className = '',
}) => {
  return (
    <div className="w-full">
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label}
      </label>
      <input
        type={type}
        name={name}
        placeholder={placeholder}
        value={value}
        onChange={onChange}
        disabled={disabled}
        className={twMerge(
          'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none',
          'text-primary',
          disabled ? 'cursor-not-allowed' : 'bg-white',
          className
        )}
      />
    </div>
  );
};

export default TextInput;
