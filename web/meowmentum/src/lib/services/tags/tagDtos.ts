import { ITag } from '@/common/tags';

export interface TagResponse extends ITag {
  id: number;
  name: string;
  createdAt: Date;
  updatedAt: Date;
  userId: number; // todo: remove from response
}