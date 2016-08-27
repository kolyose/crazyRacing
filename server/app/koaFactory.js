"use strict";

import Koa from 'koa';
import Debug from './../debug';
import route from 'koa-route';

export default async function(){
    const debug = Debug('CR:koaApp');
    const app = new Koa();
    app.context.clients = new Map();
    return app;
}