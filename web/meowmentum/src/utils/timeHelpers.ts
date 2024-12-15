export function parseTimeToMinutes(input: string): number | null {
  const timeRegex = /^(\d+h)?\s*(\d+m)?$/;
  const match = input.trim().match(timeRegex);

  if (!match) {
    return null;
  }

  const hoursMatch = match[1];
  const minutesMatch = match[2];

  const hours = hoursMatch ? parseInt(hoursMatch.replace('h', '')) : 0;
  const minutes = minutesMatch ? parseInt(minutesMatch.replace('m', '')) : 0;

  return hours * 60 + minutes;
}

export function calculateDuration(
  startDate: Date | null,
  endDate: Date | null
): string {
  if (!startDate || !endDate) {
    return '0m';
  }
  if (startDate > endDate) {
    return '0m';
  }
  const diffMilliseconds = endDate.getTime() - startDate.getTime();
  const totalMinutes = Math.floor(diffMilliseconds / (1000 * 60));
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;

  const hoursString = hours > 0 ? `${hours}h` : '';
  const minutesString = minutes > 0 ? `${minutes}m` : '';

  return [hoursString, minutesString].filter(Boolean).join(' ');
}
