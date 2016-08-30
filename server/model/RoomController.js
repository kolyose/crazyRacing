"use strict";

import Debug from './../debug';
import EventEmitter from 'events';
import {ROOM_IS_FULL, START_GAME, ADD_PLAYERS} from './constants';

const debug = new Debug('CR:RoomController');

export default (function (){

    let _id         = new WeakMap();
    let _maxClients = new WeakMap();
    let _clients    = new WeakMap();

    class RoomController extends EventEmitter{
        constructor(id, maxClients=6, clients=[]){
            super();
            _id.set(this, id);
            _maxClients.set(this, maxClients);
            _clients.set(this, clients);
        }

        get id(){
            return _id.get(this);
        }

        get maxClients(){
            return _maxClients.get(this);
        }

        get clients(){
            return _clients.get(this);
        }

        get players(){
            return Array.from(this.clients).map((client) => {return client.playerVO})
        }

        get playersIds(){
            return Array.from(this.players).map((playerVO) => {return playerVO.id})
        }

        get isFull(){
            return (this.clients.length === this.maxClients);
        }

        addClient(newClient){
            if (this.isFull) return;

            this._notifyExistingClientsAboutNewOne(newClient);

            let clients = this.clients;
            clients.push(newClient);
            _clients.set(this, clients);

            this._notifyNewClientAboutExistingOnes(newClient);

            if (this.isFull) {
                debug(`ROOM_IS_FULL`)
                this.emit(ROOM_IS_FULL, this);
            }
        }

        _notifyExistingClientsAboutNewOne(newClient){
            let data = {};
            data.players = [newClient.playerVO.toObject()];
            Array.prototype.forEach.call(this.clients, client => {
                client.socket.emit(ADD_PLAYERS, {data});
            });
        }

        _notifyNewClientAboutExistingOnes(newClient){
            let data = {};
            data.players = Array.prototype.map.call(this.clients, client => {
                return client.playerVO.toObject();
            });
            newClient.socket.emit(ADD_PLAYERS, {data});
        }

        removeClient(client){
            let clients = this.clients
            clients.splice(clients.indexOf(client), 1)
            _clients.set(this, clients)
        }
    }

    return RoomController;
}());
