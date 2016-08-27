"use strict";

import koaFactory from './koaFactory';
import ioDecorator from './socketIODecorator';
import config from './../config';
import Debug from './../debug';

async function main(){
    try {
        const debug = Debug('CR:index');
        let app = await koaFactory();
        app = await ioDecorator(app);
        app.listen(config.port, () => { debug(`Server listening on port ${config.port}`);});
    } catch (err) {
        //TODO: add logger here
    }
}

module.exports = main;