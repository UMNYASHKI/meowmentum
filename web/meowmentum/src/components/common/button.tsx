import { twMerge } from 'tailwind-merge';
import { ReactNode } from 'react';

interface ButtonProps {
  type?: 'button' | 'submit' | 'reset';
  onClick?: () => void;
  children: ReactNode;
  bordered?: boolean;
  icon?: ReactNode;
  className?: string;
  borderColor?: string;
}

const Button: React.FC<ButtonProps> = ({
  type = 'button',
  onClick,
  children,
  bordered = false,
  icon,
  className = '',
  borderColor = 'border-gray-300',
}) => {
  const baseStyles =
    'px-3 py-2 font-semibold rounded-lg focus:outline-none focus:ring-2 focus:ring-offset-2 flex items-center justify-center space-x-2';

  const borderedStyles = bordered
    ? `${borderColor} border text-gray-900 hover:bg-gray-100`
    : 'bg-black text-white hover:bg-gray-800';

  const buttonClass = twMerge(baseStyles, borderedStyles, className);

  return (
    <button type={type} onClick={onClick} className={buttonClass}>
      {icon && <span className="mr-2">{icon}</span>}
      <span>{children}</span>
    </button>
  );
};

export default Button;
