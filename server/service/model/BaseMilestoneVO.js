'use strict';

import Debug from './../../debug';
const debug = new Debug('CR:service:model:BaseMilestoneVO');

export default class BaseMilestoneVO {
  constructor() {}

  setData() {}

  getData() {
    const result = Object.create(null);
    result.type = this.type;
    return result;
  }

  get type() {
    return MILESTONE_TYPE_BASE;
  }
}

export const MILESTONE_TYPE_INIT = 0;
export const MILESTONE_TYPE_MOVEMENT = 1;
export const MILESTONE_TYPE_BOOST = 2;
export const MILESTONE_TYPE_BLOCKED = 3;
export const MILESTONE_TYPE_SUPERPOWER = 4;
