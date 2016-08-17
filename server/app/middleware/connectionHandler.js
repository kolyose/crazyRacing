import Debug from './../../debug';

export default function (socket){
    const debug = new Debug('connectionHandler');
    debug(`CONNECTED ${socket}`);
    //console.dir(socket);
    console.dir(this);

    socket.on('join', data => {
        debug(`DATA ${data}`);
    });
}
