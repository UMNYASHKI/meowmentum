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
      query: (credentials) => ({
        url: `/${endpointRoute}?id=${credentials?.id?.toString() || ''}`,
        method: 'POST',
        body: credentials,
      }),
    }),
    getTask: builder.query<TaskResponse[], TaskFilterRequest>({
      query: (filterRequest) => {
        let queryString = '';
        queryString +=
          filterRequest.taskId !== undefined
            ? 'taskId=' + filterRequest.taskId + '&'
            : '';
        queryString +=
          filterRequest.tagIds.length > 0
            ? filterRequest.tagIds.map((x) => 'tagIds=' + x + '&').join()
            : '';
        queryString +=
          filterRequest.priorities.length > 0
            ? filterRequest.priorities.map((x) => 'priorities=' + x + '&').join()
            : '';
        queryString +=
          filterRequest.status.length > 0
            ? filterRequest.status.map((x) => 'status=' + x + '&').join()
            : '';
        const token = localStorage.getItem('token');
        console.log(`/${endpointRoute}?` + queryString);
        return {
          url: `/${endpointRoute}?` + queryString,
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
