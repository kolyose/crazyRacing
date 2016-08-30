"use strict";

import Debug from './../debug';

export default (function(){

    const debug = new Debug("CR:ClientProxy");

    let _socket     = new WeakMap();
    let _playerVO   = new WeakMap();

    class ClientProxy{
        constructor(socket, playerVO){
            _socket.set(this, socket);
            _playerVO.set(this, playerVO);
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

            //after refactoring there is no need in this method anymore as
            //all joining to room functionality moved to RoomsManager
            //all clients notification functionality moved to RoomController

            //join player to room through RoomsManager
            /*const room = RoomsManager.joinClientToRoomByID(roomID, this);

            //notify other players in the room about new player joined
            let data = [this.playerVO.toObject()];
            this.socket.to(room.id).emit(ADD_PLAYERS, {data});

            //join the player within io engine
            this.socket.join(room.id);

            data = Array.prototype.map.call(room.clients, client => {
                return client.playerVO.toObject();
            });

            debug(`ADD_PLAYERS->data:${data}`);

            //notify the player about other players already joined to the room (including himself)
            this.socket.emit(ADD_PLAYERS, {data});*/
        }
    }

    return ClientProxy;

})()