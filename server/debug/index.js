import Debug from 'debug';

let _debuggersByScope = {};

export default function (scope) {
   return function debug(message, data){
       if (process.env.DEBUG){
           if (!_debuggersByScope[scope]){
               _debuggersByScope[scope] = new Debug(scope);
           }
           _debuggersByScope[scope](message);
           if (data){
               console.dir(data);
           }
           return;
       }

       console.log(`${scope}: ${message}`);
       if (data){
           console.dir(data);
       }

   }
}
