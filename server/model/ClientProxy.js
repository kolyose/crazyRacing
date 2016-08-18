"use strict";

import {ADD_PLAYERS, START_GAME} from './constants';
import Debug from './../debug';

export default (function(){

    const debug = new Debug("ClientProxy");

    let _socket     = new WeakMap();
    let _playerVO   = new WeakMap();
    let _context   = new WeakMap();

    class ClientProxy{
        constructor(socket, playerVO, context){
            _socket.set(this, socket);
            _playerVO.set(this, playerVO);
            _context.set(this, context);
            return this;
        }

        get socket(){
            return _socket.get(this);
        }

        get id(){
            return this.playerVO.id;
        }

        get playerVO(){
            return _playerVO.get(this);
        }

        get context(){
            return _context.get(this);
        }

        joinToRoom(roomID){

            //adding player to hash of active players (the ones have joined any rooms)
            this.context.clients.set(this.socket, this);

            //join player to room through RoomsManager
            const room = this.context.roomsManager.joinClientToRoomByID(roomID, this);

            debug(`joinToRoom-> roomID ${roomID}`);
            debug(`joinToRoom->room: ${room}`);

            //notify other players in the room about new player joined
            let data = this.playerVO;
            debug(`data ${data}`);
            //debug(`player name: ${this.playerVO.name} player id: ${this.playerVO.id}`)
            this.socket.to(room.id).emit(ADD_PLAYERS, {data:"HELLO"});

            //join the player within io engine
            this.socket.join(room.id);

            data = [].map.call(room.clients, client => {return client.playerVO;});
            debug(`data for new player: ${data}`)


            //notify the player about other players already joined to the room (including himself)
            this.socket.emit(ADD_PLAYERS, {"data":"GOOD BYE"});

            if (room.isFull){
                this.socket.to(room.id).emit(START_GAME);
            }
        }
    }

    return ClientProxy;

})();