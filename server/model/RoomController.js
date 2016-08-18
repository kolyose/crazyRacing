"use strict";

import EventEmitter from 'events';

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

        get isFull(){
            return (this.clients.length === this.maxClients);
        }

        addClient(newClient){
            if (this.isFull) return;
            _clients.set(this, [].push.call(this.clients, newClient));
        }

        removeClient(client){
            let clients = this.clients;
            clients.splice(clients.indexOf(client), 1);
            _clients.set(this, clients);
        }
    }

    return RoomController;
}());
