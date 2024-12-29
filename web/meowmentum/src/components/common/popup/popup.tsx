import React from 'react';
import './popup.css';

interface PopupProps {
  message: string;
}

const Popup = ({ message }: PopupProps) => {
  return (
    <>
      <div className="popup">{message}</div>
    </>
  );
};

export default Popup;
