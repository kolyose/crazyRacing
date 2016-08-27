"use strict";

export default (function () {

    let _direction = new WeakMap();
    let _boost = new WeakMap();

    class PlayerActionsVO{
        constructor({direction, boost}){
            _direction.set(this, direction);
            _boost.set(this, boost);
        }

        get direction(){
            return _direction.get(this);
        }

        get boost(){
            return _boost.get(this);
        }
    }

    return PlayerActionsVO;

})();