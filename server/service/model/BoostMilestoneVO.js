"use strict";

import {MILESTONE_TYPE_BOOST} from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BoostMilestoneVO');

let _speed = new WeakMap();

export default class BoostMilestoneVO extends MovementMilestoneVO{
    constructor(){
        super();
        debug(`MILESTONE_TYPE_BOOST created`);
    }

    setData({x,y,speed}){
        super.setData({x,y});
        this.speed = speed;
    }

    getData(){
        const obj = super.getData();
        obj.speed = this.speed;
        return obj;
    }    

    get type(){
        debug(`get type BoostMilestoneVO`)
        return MILESTONE_TYPE_BOOST;
    }

    set speed(value){
        _speed.set(this, value);
    }

    get speed(){
        return _speed.get(this);
    }
}