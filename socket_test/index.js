/**
 * Created by kolyos on 13.08.2016.
 */
var Koa = require('koa');
var app = new Koa();

var IO = require('koa-socket');
var io = new IO();
io.attach(app);

io.on('connection', (ctx, socket) => {
    "use strict";
    console.log('CONNECTION');
    console.log('socket: ' + socket);

    ctx.socket.on('data', (data) => {
        "use strict";
        console.log('DATA 1');
        console.dir(data);
    });
});

io.on('data', (ctx, data) => {
    "use strict";
    console.log('DATA 2');
    console.log('ctx: ');
    console.dir(ctx);
    console.log('socket: ');
    console.dir(data);
});


app.listen(3000, () => {
    "use strict";
    console.log("listening on port 3000");
})


/*
 var http = require('http').Server(app.callback());
 var io = require('socket.io')(http);

 io.on('connection', (socket) => {
    "use strict";
    console.log('CONNECTION!!!');
    console.log('socket: ' + socket);

    socket.on('data', (data) => {
        "use strict";
        console.log('MESSAGE!!!');
        console.log('data: ' + data.message);
    });
});

http.listen(3000, () => {
    "use strict";
    console.log("listening on port 3000");
})*/