import Debug from './../../debug';
import {JOIN_ROOM, USER_ACTIONS} from './../../model/constants';
import PlayerVO from './../../model/PlayerVO';
import ClientProxy from './../../model/ClientProxy';

export default function (socket){

    //to get an id use following:
    //socket.id

    //to get a hash (by socket id) of connected sockets use following:
    //socket.nsp.sockets
    //or
    //socket.nsp.connected
    //(what is the difference?)

    //to get a number of connected clients/sockets use following:
    //socket.server.eio.clientsCount
    //or
    //socket.server.httpServer._connections
    //or
    //socket.server.engine.clientsCount

    //to get a hash (by socket id) of existing rooms use following:
    //socket.adapter.rooms
    //or
    //socket.adapter.sids

    const debug = new Debug('connectionHandler');
    debug(`CONNECTED ${socket}`);

    socket.on(JOIN_ROOM, data => {
        debug(`JOIN_ROOM ${data}`);
        //console.dir(data);
        const client = new ClientProxy(socket, new PlayerVO(data.playerVO), this.context);
        client.joinToRoom(data.roomID);
    });

    socket.on(USER_ACTIONS, data => {
        debug(`USER_ACTIONS ${data}`);

    });
}
