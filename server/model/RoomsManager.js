"use strict";

import {MAX_ROOM_PLAYERS, ROOM_IS_FULL} from './constants';
import RoomController from './RoomController';
import GameplayController from './GameplayController';
import Debug from './../debug';

let _publicRooms = new WeakMap()
const debug = new Debug("CR:RoomsManager");

class RoomsManager{
    constructor(){
        _publicRooms.set(this, []/*array of RoomVO*/);
    }

    get publicRooms(){
        return _publicRooms.get(this);
    }

    addPublicRoom(roomID, maxClients=MAX_ROOM_PLAYERS, clients=[]){
        const newRoom = new RoomController(roomID, maxClients, clients);
        newRoom.on(ROOM_IS_FULL, this._startGameInRoom);
        this.publicRooms.push(newRoom);
        return newRoom;
    }

    _startGameInRoom(room) {
        let gameplayController = new GameplayController(room);
        gameplayController.init();
        gameplayController.start();
    }

    joinClientToAvailablePublicRoom(client){
        const rooms = this.publicRooms;
        let room = rooms[rooms.length-1];

        if (!rooms.length || room.isFull){
            room = this.addPublicRoom(client.id);
        }

        room.addClient(client);
        //debug(`joinClientToAvailablePublicRoom with id:${room.id}`);
        return room;
    }

    joinClientToRoomByID(roomID, client){
        if (!roomID || roomID === ''){ //if roomID is undefined it means we need to add the client to public room
           return this.joinClientToAvailablePublicRoom(client);
        }

        //if roomID is defined it means we need to add the client to private room
        //TODO: add private rooms functionality
    }
}

export default new RoomsManager();