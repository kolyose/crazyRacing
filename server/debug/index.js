import Debug from 'debug';

let _debuggersByScope = {};

export default function (scope) {
   return function (message){
       if (process.env.DEBUG){

           if (!_debuggersByScope[scope]){
               _debuggersByScope[scope] = new Debug(scope);
           }

           return _debuggersByScope[scope](message);
       }

       console.log(`${scope}: ${message}`);
   }
}
