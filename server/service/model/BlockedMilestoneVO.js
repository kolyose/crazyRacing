"use strict";

import {MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BlockedMilestoneVO');

const _blockerId = new WeakMap();

export default class BlockedMilestoneVO extends MovementMilestoneVO{
    constructor(){
        super();
    }

    setData(x,y,blockerId){
        super.setData(x,y);
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