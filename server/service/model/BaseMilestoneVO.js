"use strict";

import Debug from './../../debug';
const debug = new Debug('CR:service:model:BaseMilestoneVO');

export default class BaseMilestoneVO{
    constructor(){  
        debug(`MILESTONE_TYPE_BASE created`);
    }

    getData(){
        return {type:this.type};
    }

    get type(){
        return MILESTONE_TYPE_BASE;
    }  
}

export const MILESTONE_TYPE_BASE = 0;
export const MILESTONE_TYPE_MOVEMENT = 1;
export const MILESTONE_TYPE_BOOST = 2;
export const MILESTONE_TYPE_BLOCKED = 3;

