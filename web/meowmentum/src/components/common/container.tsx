import { twMerge } from 'tailwind-merge';

type ContainerProps = {
  children: React.ReactNode;
  className?: string;
} & React.HTMLProps<HTMLDivElement>;

export default function Container({
  children,
  className,
  ...props
}: ContainerProps) {
  const mergedClassName = twMerge(
    'w-full max-w-md bg-gray-200 rounded-lg shadow-md p-8',
    className
  );

  return (
    <div className="flex items-center justify-center h-screen">
      <div className={mergedClassName} {...props}>
        {children}
      </div>
    </div>
  );
}
