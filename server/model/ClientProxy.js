"use strict";

import {ADD_PLAYERS, START_GAME} from './constants'
import RoomsManager from './RoomsManager'
import Debug from './../debug'

export default (function(){

    const debug = new Debug("CR:ClientProxy")

    let _socket     = new WeakMap()
    let _playerVO   = new WeakMap()

    class ClientProxy{
        constructor(socket, playerVO){
            _socket.set(this, socket)
            _playerVO.set(this, playerVO)
            return this
        }

        get socket(){
            return _socket.get(this)
        }

        get id(){
            return this.playerVO.id
        }

        get playerVO(){
            return _playerVO.get(this)
        }

        get context(){
            return _context.get(this)
        }

        joinToRoom(roomID){

            //adding player to hash of active players (the ones have joined any rooms)
            this.context.clients.set(this.socket, this)

            //join player to room through RoomsManager
            const room = RoomsManager.joinClientToRoomByID(roomID, this)

            //notify other players in the room about new player joined
            let data = [this.playerVO.toObject()]
            this.socket.to(room.id).emit(ADD_PLAYERS, {data})

            //join the player within io engine
            this.socket.join(room.id)

            data = Array.prototype.map.call(room.clients, client => {
                return client.playerVO.toObject()
            });

            //notify the player about other players already joined to the room (including himself)
            this.socket.emit(ADD_PLAYERS, {data})

            //if (room.isFull){
            //    this.socket.to(room.id).emit(START_GAME)
            //}
        }
    }

    return ClientProxy

})()