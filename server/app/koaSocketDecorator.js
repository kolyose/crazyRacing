import IO from 'koa-socket';
import Debug from './../debug';
import connectionHandler from './middleware/connectionHandler';

export default async function (app){
   const debug = Debug('io');
   const io = new IO();
   io.attach(app);

   io.on('connection', connectionHandler.bind(app._io));

   io.on('data', (ctx, data) => {
        "use strict";
        debug(`DATA ${data}`);
        console.dir(data);
    });

    return app;
}
