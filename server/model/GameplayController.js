import {FIELD_WIDTH, PLAYER_ACTIONS, START_GAME, ROUND_RESULTS} from './constants';
import ComputingService from './../service/ComputingService';
import PlayerActionsVO from './PlayerActionsVO';
import Util from './../util';
import EventEmitter from 'events';
import Debug from  './../debug';

const debug = new Debug('CR:GameplayController');
const hasOwnProperty = Object.prototype.hasOwnProperty;

export default (function(){

    const _room = new WeakMap();
    const _receivedActionsCounter = new WeakMap();
    const _actionsByPlayerId = new WeakMap();
    const _positionsByPlayerId = new WeakMap();
    const _distancesByPlayerId = new WeakMap();

    class GameplayController extends EventEmitter{
        constructor(room){
            super();
            _room.set(this, room);
            _positionsByPlayerId.set(this, {});
            this._resetRoundData();
        }

        get room(){
            return _room.get(this);
        }

        init(){
            //initializing array of racetrack numbers for further initialization of players' random positions
            let raceTrackNumbers = [];
            for (let i=0; i<FIELD_WIDTH; i++){
                raceTrackNumbers.push(i);
            }

            for (let client of this.room.clients){
                //setting random initial racetrack position for each player
                this._setPositionByPlayerId(client.playerVO.id, {x:0, y:Util.getRandomValueFromArray(raceTrackNumbers)});

                client.socket.on(PLAYER_ACTIONS, async (data) => {
                    this._setActionsByPlayerId(client.playerVO.id, new PlayerActionsVO(data));
                    debug(`PLAYER_ACTIONS`)
                    const counter = this._increaseReceivedActionsCounter();
                    if (counter == this.room.maxClients){
                        let dataToCompute = this._getDataToCompute();
                        const milestonesByPlayerId = await ComputingService(dataToCompute);
                        this._updatePlayersPositions(milestonesByPlayerId);
                        this._resetRoundData();
                        const  roundResultsData = this._getRoundResultsData(milestonesByPlayerId);
                        this._broadcastEventToClients(ROUND_RESULTS, roundResultsData);
                    }
                })
            }
        }

        start(){
            debug(`START`)
            let milestonesByPlayerId = {};
            for (let playerId of this.room.playersIds){
                const position = this._getPositionByPlayerId(playerId);
                milestonesByPlayerId[playerId] = [position];
            }
            const initialData = this._getRoundResultsData(milestonesByPlayerId);
           // debug(`START_GAME with initialData: `, initialData)
            this._broadcastEventToClients(START_GAME, initialData);
        }

        _getRoundResultsData(computedResults){
            //creating a final data structure for clients
            const data = {results:[]};
            //and filling the data with final results per player
            for (let playerId in computedResults) if (hasOwnProperty.call(computedResults, playerId)){
                const playerResults = computedResults[playerId];
                //if there is no place prop in player's results data meaning the player has not finished the race yet
                //we need to decorate the data with random distance property for next round
                if (!playerResults.place){
                    playerResults.distance = this._getRandomDistanceByPlayerId(playerId)
                }
                data.results.push(playerResults);
            }
            return data;
        }

        _broadcastEventToClients(event, data){
            for (let client of this.room.clients){
                client.socket.emit(event, {data});
            }
        }

        _resetRoundData(){
            _receivedActionsCounter.set(this, 0);
            _actionsByPlayerId.set(this, {});
            _distancesByPlayerId.set(this, {});
        }

        _increaseReceivedActionsCounter(){
            let counter = _receivedActionsCounter.get(this);
            counter++;
            _receivedActionsCounter.set(this, counter);
            return counter;
        }

        _getDataToCompute(){
            let data = {};
            for (let client of this.room.clients){
                const playerId = client.playerVO.id;
                data[playerId] = {
                    position: this._getPositionByPlayerId(playerId),
                    actions: this._getActionsByPlayerId(playerId),
                    distance: this._getDistanceByPlayerId(playerId)
                }
            }
            return data;
        }

        _getRandomDistanceByPlayerId(playerId){
            const playerPosition = this._getPositionByPlayerId(playerId);
            const randomDistance =  this._calculateRandomDistanceByPosition(playerPosition.y);
            this._setDistanceByPlayerId(playerId, randomDistance);
            return randomDistance;
        }

        _calculateRandomDistanceByPosition(position){
            let randomDistance;
            switch(position){
                case 0:{
                    randomDistance = Math.floor(Math.random()*4) + 1;
                    break
                }
                case 1:{
                    randomDistance = Math.floor(Math.random()*3) + 2;
                    break
                }
                case 2:{
                    randomDistance = Math.floor(Math.random()*4) + 2;
                    break
                }
                case 3:{
                    randomDistance = Math.floor(Math.random()*3) + 3;
                    break
                }
                case 4:{
                    randomDistance = Math.floor(Math.random()*4) + 3;
                    break
                }
                case 5:{
                    randomDistance = Math.floor(Math.random()*3) + 4;
                    break
                }
                default: randomDistance = 0;
            }
            return randomDistance;
        }

        _updatePlayersPositions(milestonesByPlayerId){
            for (let playerId in milestonesByPlayerId) if (hasOwnProperty.call(milestonesByPlayerId, playerId)) {
                const milestones = milestonesByPlayerId[playerId];
                const finalMilestone = milestones[milestones.length-1];
                this._setPositionByPlayerId(playerId, {x:finalMilestone.x, y:finalMilestone.y});
            }
        }

        _getPositionByPlayerId(playerId){
            let positions = _positionsByPlayerId.get(this);
            const position = positions[playerId];
            return position;
        }

        _setPositionByPlayerId(playerId, newPosition){
            let positions = _positionsByPlayerId.get(this);
            positions[playerId] = newPosition;
            _positionsByPlayerId.set(this, positions);
        }

        _getActionsByPlayerId(playerId){
            let actions = _actionsByPlayerId.get(this);
            return actions[playerId];
        }

        _setActionsByPlayerId(playerId, newActions){
            let actions = _actionsByPlayerId.get(this);
            actions[playerId] = newActions;
            _actionsByPlayerId.set(this, actions);
        }

        _getDistanceByPlayerId(playerId){
            let distances = _distancesByPlayerId.get(this);
            return distances[playerId];
        }

        _setDistanceByPlayerId(playerId, newDistance){
            let distances = _distancesByPlayerId.get(this);
            distances[playerId] = newDistance;
            _distancesByPlayerId.set(this, distances);
        }
    }

    return GameplayController;
})()