export default function Dashboard({ children }: { children: React.ReactNode }) {
  return (
    <>
      <div className="flex w-[80%] bg-[#FAFAFA]">{children}</div>
    </>
  );
}
