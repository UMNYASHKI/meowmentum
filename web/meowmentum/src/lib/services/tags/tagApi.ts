import { createApi } from '@reduxjs/toolkit/query/react';
import baseAuthQuery from '@services/helpers/baseAuthQuery';
import { TagResponse } from '@services/tags/tagDtos';

const name = 'tagApi';
const endpointRoute: string =
  process.env.NODE_ENV !== 'production' ? 'api/tag' : 'core/api/tag';

export const tagApi = createApi({
  reducerPath: name,
  baseQuery: baseAuthQuery,
  endpoints: (builder) => ({
    getAllTags: builder.query<TagResponse[], void>({
      query: (credentials) => ({
        url: `/${endpointRoute}`,
        method: 'GET',
      }),
    }),
  }),
});

export const { useLazyGetAllTagsQuery } = tagApi;
