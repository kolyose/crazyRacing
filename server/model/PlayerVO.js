"use strict";

import Debug from './../debug';
const debug = new Debug('CR:PlayerVO');

export default (function(){

    let _id          = new WeakMap();
    let _name        = new WeakMap();
    let _characterId = new WeakMap();

    class PlayerVO{
        constructor({id, name, characterData: {characterId}}){
            debug(`PlayerVO id=${id} name=${name} characterId=${characterId}`);
            _id.set(this, id);
            _name.set(this, name);
            _characterId.set(this, characterId);
        }

        get id(){
            return _id.get(this);
        }

        get name(){
            return _name.get(this);
        }
        
        get characterId(){
            return _characterId.get(this);
        }

        toObject(){
            return {id:this.id, name:this.name, characterData:{characterId: this.characterId}};
        }
    }

    return PlayerVO;

})();