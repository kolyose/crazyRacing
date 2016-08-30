"use strict";

import Debug from './../debug';
const debug = new Debug('CR:PlayerVO');

export default (function(){

    let _id          = new WeakMap();
    let _name        = new WeakMap();
    let _characterID = new WeakMap();

    class PlayerVO{
        constructor({id, name}){
            debug(`PlayerVO id=${id} name=${name}`);
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