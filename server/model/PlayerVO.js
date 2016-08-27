"use strict";

export default (function(){

    let _id          = new WeakMap();
    let _name        = new WeakMap();
    let _characterID = new WeakMap();

    class PlayerVO{
        constructor({id, name}){
            _id.set(this, id);
            _name.set(this, name);
        }

        get id(){
            return _id.get(this);
        }

        get name(){
            return _name.get(this);
        }

        toObject(){
            return {id:this.id, name:this.name};
        }
    }

    return PlayerVO;

})();