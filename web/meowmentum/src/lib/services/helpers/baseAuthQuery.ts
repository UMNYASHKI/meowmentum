import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseAuthQuery = fetchBaseQuery({
  baseUrl: process.env.NEXT_PUBLIC_API_URL,
  prepareHeaders: (headers) => {
    headers.set('accept', 'application/json');

    const token = localStorage.getItem('token');
    if (token) {
      headers.set('Authorization', `Bearer ${token}`); // Attach token to headers
    }

    return headers;
  },
});

export default baseAuthQuery;
