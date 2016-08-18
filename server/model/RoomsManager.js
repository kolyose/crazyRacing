"use strict";

import {MAX_ROOM_PLAYERS} from './constants';
import RoomController from './RoomController';
import Debug from './../debug';

export default (function(){

    let _publicRooms = new WeakMap();
    const debug = new Debug("RoomsManager");

    class RoomsManager{
        constructor(){
            _publicRooms.set(this, []/*array of RoomVO*/);
        }

        get publicRooms(){
            return _publicRooms.get(this);
        }

        addPublicRoom(roomID, maxClients=MAX_ROOM_PLAYERS, clients=[]){
            const newRoom = new RoomController(roomID, maxClients, clients);
            this.publicRooms.push(newRoom);

            debug(`addPublicRoom -> roomID ${roomID}`);
            debug(`addPublicRoom -> newRoom ${newRoom}`);

            return newRoom;
        }

        joinClientToAvailablePublicRoom(client){
            const rooms = this.publicRooms;
            let room = rooms[rooms.length-1];

            debug(`joinClientToAvailablePublicRoom -> room 1 ${room}`);

            if (!rooms.length || room.isFull){
                room = this.addPublicRoom(client.id);
                room.addClient(client);

                debug(`joinClientToAvailablePublicRoom -> room 2 ${room}`);

            } else {
                room.addClient(client);
            }
            return room;
        }

        joinClientToRoomByID(roomID, client){
            if (!roomID || roomID === ''){ //if roomID is undefined it means we need to add the client to public room
                debug(`joinClientToRoomByID`);
                return this.joinClientToAvailablePublicRoom(client);
            }

            //if roomID is defined it means we need to add the client to private room
            //TODO: add private rooms functionality
        }
    }

    return RoomsManager;

})();