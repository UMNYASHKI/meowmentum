import { createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@services/helpers/baseAuthQuery';
import {
  AddTimeIntervalRequest,
  TimeIntervalResponse,
  UpdateTimeIntervalRequest,
} from '@services/timeIntervals/timeIntervalsDtos';

const name = 'timeIntervalsApi';
const endpointRoute: string =
  process.env.NODE_ENV !== 'production' ? 'api/timer' : 'core/api/timer';

export const timeIntervalsApi = createApi({
  reducerPath: name,
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    deleteInterval: builder.query<void, number>({
      query: (credentials) => ({
        url: `${endpointRoute}/${credentials}`,
        method: 'DELETE',
      }),
    }),
    addInterval: builder.mutation<number, AddTimeIntervalRequest>({
      query: (credentials) => ({
        url: `${endpointRoute}/log`,
        method: 'POST',
        body: credentials,
      }),
    }),
    updateInterval: builder.mutation<void, UpdateTimeIntervalRequest>({
      query: (credentials) => ({
        url: `${endpointRoute}/${credentials?.id}`,
        method: 'PUT',
        body: credentials,
      }),
    }),
    getAllTaskIntervals: builder.query<TimeIntervalResponse[], number>({
      query: (credentials) => ({
        url: `${endpointRoute}?taskId=${credentials}`,
        method: 'GET',
      }),
    }),
  }),
});

export const {
  useLazyDeleteIntervalQuery,
  useUpdateIntervalMutation,
  useAddIntervalMutation,
  useLazyGetAllTaskIntervalsQuery,
} = timeIntervalsApi;
