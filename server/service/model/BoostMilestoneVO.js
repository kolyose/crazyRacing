"use strict";

import {MILESTONE_TYPE_BOOST} from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:BoostMilestoneVO');

let _speed = new WeakMap();

export default class BoostMilestoneVO extends MovementMilestoneVO{
    constructor(){
        super();
    }

    setData(x,y){
        super.setData(x,y);
    }

    getData(){
        const obj = super.getData();
        return obj;
    }    

    get type(){
        return MILESTONE_TYPE_BOOST;
    }

     get speed(){
        return 2;
    }
}