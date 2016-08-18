"use strict";

import Koa from 'koa';
import Debug from './../debug';
import route from 'koa-route';
import RoomsManager from './../model/RoomsManager';

export default async function(){
    const debug = Debug('koaApp');
    const app = new Koa();
    app.context.roomsManager = new RoomsManager();
    app.context.clients = new Map();
    return app;
}