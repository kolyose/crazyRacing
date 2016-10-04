"use strict";

import {MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import BaseMilestoneVO from './BaseMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BlockedMilestoneVO');

const _blockerId = new WeakMap();

export default class BlockedMilestoneVO extends BaseMilestoneVO{
    constructor(){
        super();
    }

    setData(blockerId){
        this.blockerId = blockerId;
    }

    getData(){
        const obj = super.getData();
        obj.blockerId = this.blockerId;
        return obj;
    }

    get type(){
        return MILESTONE_TYPE_BLOCKED;
    }

    set blockerId(value){
        _blockerId.set(this, value);
    }

    get blockerId(){
        return _blockerId.get(this);
    }
}