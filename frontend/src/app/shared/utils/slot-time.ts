export const SLOT_START_HOUR = 8;
export const SLOT_MINUTES = 30;
export const DAY_SLOT_COUNT = 20;
export const MAX_RESERVATION_SLOTS = 4;

export function getTodayDateInputValue(now = new Date()): string {
  const year = now.getFullYear();
  const month = `${now.getMonth() + 1}`.padStart(2, '0');
  const day = `${now.getDate()}`.padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function slotIndexToTime(slotIndex: number): string {
  const totalMinutes = SLOT_START_HOUR * 60 + slotIndex * SLOT_MINUTES;
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;
  return `${`${hours}`.padStart(2, '0')}:${`${minutes}`.padStart(2, '0')}`;
}

export function durationFromSlots(durationSlots: number): string {
  const minutes = durationSlots * SLOT_MINUTES;
  if (minutes === 60) return '1 hour';
  if (minutes % 60 === 0) return `${minutes / 60} hours`;
  return `${minutes / 60} hours`;
}

export function durationFromMinutes(durationMinutes: number): string {
  if (durationMinutes === 60) return '1 hour';
  if (durationMinutes % 60 === 0) return `${durationMinutes / 60} hours`;
  return `${durationMinutes / 60} hours`;
}
