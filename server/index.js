"use strict";

require('babel-register');
require('babel-polyfill');

const app = require('./app');
const config = require('./config');
const debug = require('debug')('index');

app.then(app => {
        app.listen(config.port);
        debug('Server listening...');
    })
    .catch(err => {
        //TODO: add logger here
    })
