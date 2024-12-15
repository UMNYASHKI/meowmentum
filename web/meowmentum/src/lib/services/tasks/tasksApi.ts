import { BaseQueryArg, createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@services/helpers/baseAuthQuery';
import {
  CreateTaskRequest,
  TaskFilterRequest,
  TaskResponse,
} from '@services/tasks/tasksDtos';

const name = 'tasksApi';
const endpointRoute: string =
  process.env.NODE_ENV !== 'production' ? 'api/tasks' : 'core/api/tasks';

export const tasksApi = createApi({
  reducerPath: name,
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    createTask: builder.mutation<boolean, CreateTaskRequest>({
      query: (credentials) => {
        const token = localStorage.getItem('token');
        console.log(token);
        return {
          url: `/${endpointRoute}`,
          method: 'POST',
          body: credentials,
          headers: {
            Authorization: `Bearer ${token}`,
          },
        };
      },
    }),
    getTask: builder.query<TaskResponse[], TaskFilterRequest>({
      query: (filterRequest) => {
        // const queryString = new URLSearchParams({
        //   taskId: filterRequest.taskId?.toString() || '',
        //   status: filterRequest.status?.join(',') || '',
        //   tagIds: filterRequest.tagIds?.join(',') || '',
        //   priorities: filterRequest.priorities?.join(',') || '',
        // }).toString();
        const token = localStorage.getItem('token');
        return {
          url: `/${endpointRoute}?${''}`,
          method: 'GET',
          headers: {
            Authorization: 'Bearer' + token,
          },
        };
      },
    }),
  }),
});

export const { useCreateTaskMutation, useLazyGetTaskQuery } = tasksApi;
