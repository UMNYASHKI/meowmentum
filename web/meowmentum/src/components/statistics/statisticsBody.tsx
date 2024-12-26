'use client';

import React, { useEffect, useState } from 'react';
import { Spinner } from '@nextui-org/react';
import * as XLSX from 'xlsx';
import './style.css';

interface StatisticsBodyProps {
  isLoading: boolean;
  error: string | null;
  iframeLink: string | null; // External URL or Blob URL
}

export default function StatisticsBody({
  isLoading,
  error,
  iframeLink,
}: StatisticsBodyProps) {
  const [excelContent, setExcelContent] = useState<string | null>(null);

  const parseExcelFile = async (blob: Blob) => {
    const reader = new FileReader();
    reader.onload = (event) => {
      const data = new Uint8Array(event.target?.result as ArrayBuffer);
      const workbook = XLSX.read(data, { type: 'array' });
      const firstSheetName = workbook.SheetNames[0];
      const worksheet = workbook.Sheets[firstSheetName];
      const html = XLSX.utils.sheet_to_html(worksheet);
      setExcelContent(html);
    };
    reader.readAsArrayBuffer(blob);
  };

  useEffect(() => {
    if (iframeLink) {
      fetch(iframeLink)
        .then((response) => response.blob())
        .then((blob) => parseExcelFile(blob))
        .catch((err) => console.error('Error parsing Excel file:', err));
    }
  }, [iframeLink]);

  return (
    <div className="m-5 max-w-7xl">
      {isLoading ? (
        <div className="flex justify-center items-center h-64">
          <Spinner size="lg" />
        </div>
      ) : excelContent ? (
        <div
          className="excel-content max-h-[calc(100vh-250px)] w-full max-w-full overflow-auto relative border rounded shadow-sm"
          dangerouslySetInnerHTML={{ __html: excelContent }}
        />
      ) : (
        <div className="text-center mt-5 font-medium text-black text-xl">
          {error ? <p>{error}</p> : <p>No report available</p>}
        </div>
      )}
    </div>
  );
}
