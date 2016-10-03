"use strict";

import {MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BlockedMilestoneVO');

export default class BlockedMilestoneVO extends MovementMilestoneVO{
    constructor(){
        super();
        debug(`MILESTONE_TYPE_BLOCKED created`);
    }

    setData({x,y}){
        super.setData({x,y});
    }

    getData(){
        const obj = super.getData();
        return obj;
    }

    get type(){
        debug(`get type BlockedMilestoneVO`)
        return MILESTONE_TYPE_BLOCKED;
    }
}