"use strict";

import BaseMilestoneVO from './BaseMilestoneVO';
import {MILESTONE_TYPE_MOVEMENT} from './BaseMilestoneVO';
import Debug from './../../debug';
const debug = new Debug('CR:service:model:MovementMilestoneVO');

let _x = new WeakMap();
let _y = new WeakMap();

export default class MovementMilestoneVO extends BaseMilestoneVO{
    constructor(){ 
         super();  
    }

    setData(x,y){
        this.x = x;
        this.y = y;
    }

    getData(){
        const obj = super.getData();
        obj.x = this.x;
        obj.y = this.y;
        obj.speed = this.speed;
        return obj;
    }

    get type(){
        return MILESTONE_TYPE_MOVEMENT;
    }

    set x(value){
        _x.set(this, value);
    }

    get x(){
        return _x.get(this);
    }

    set y(value){
        _y.set(this, value);
    }

    get y(){
        return _y.get(this);
    }

    get speed(){
        return 1;
    }
}