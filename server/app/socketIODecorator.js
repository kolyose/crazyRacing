import Http from 'http';
import IO from 'socket.io';
import Debug from './../debug';
import connectionHandler from './middleware/connectionHandler';

export default async function (app){
    const debug = Debug('CR:io');
    const http = Http.Server(app.callback());
    const io = IO(http);
    app.context.io = io;
    io.on('connection', connectionHandler.bind(app));
    return http;
}