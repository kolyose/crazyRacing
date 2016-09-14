"use strict";

export default (function () {

    let _direction = new WeakMap();
    let _boost = new WeakMap();

    class PlayerActionsVO{
        constructor({direction, boost}){
            this.direction = direction;
            this.boost = boost;
        }

        set direction(value){
            _direction.set(this, value);
        }

        get direction(){
            return _direction.get(this);
        }

        set boost(value){
            _boost.set(this, value);
        }

        get boost(){
            return _boost.get(this);
        }

        toString(){
            return {direction: this.direction, boost: this.boost}.toString();
        }
    }

    return PlayerActionsVO;

})();