export type PopupMessageType = 'error' | 'success';
export type PopupMessage = {
  message: string;
  isVisible: boolean;
  type: PopupMessageType | null;
};
