"use strict";

import {MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import BaseMilestoneVO from './BaseMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BlockedMilestoneVO');

export default class BlockedMilestoneVO extends BaseMilestoneVO{
    constructor(){
        super();
    }

    getData(){
        const obj = super.getData();
        return obj;
    }

    get type(){
        return MILESTONE_TYPE_BLOCKED;
    }
}