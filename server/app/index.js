"use strict";

import Koa from 'koa';
import IO from 'koa-socket';
import Debug from 'debug';
import route from 'koa-route';

async function main(){
    const debug = new Debug('app');
    const app = new Koa();
    const io = new IO();
    io.attach(app);

    let connections = [];

    app.use(route.head('/', async (ctx, next) => {
        ctx.status = 200;
    }));

    app.use(route.post('/socket.io', async (ctx, next) => {
        debug('connection!!!');
        console.log("connection");
    }));


    app.io.on('connection', (ctx, socket) => {
        connections.push(socket);
        debug('connection!!!');
        console.log("connection");

        socket.on('data', (ctx, data) => {
            debug(`data {%s}`, data);
            console.log("data received: " + data.toString());
        });
    })

    app._io.on('connection', (ctx, socket) => {
        connections.push(socket);
        debug('connection!!!');
        console.log("connection");
    })

    return app;
}

module.exports = main();