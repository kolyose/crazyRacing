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
    const _boostedPlayerIds = new WeakMap();

    class GameplayController extends EventEmitter{
        constructor(room){
            super();
            _room.set(this, room);
            _positionsByPlayerId.set(this, {});
            _boostedPlayerIds.set(this, {});
            this._resetRoundData();
        }

        get room(){
            return _room.get(this);
        }

        init(){
            //initializing array of numbers of racetracks available for players
            let raceTrackNumbers = [];
            for (let i=0, length=this.room.clients.length; i<length; i++){
                raceTrackNumbers.push(FIELD_WIDTH - 1 - i);
            }

            for (let client of this.room.clients){
                //setting random initial racetrack position for each player
                this._setPositionByPlayerId(client.playerVO.id, {x:0, y:Util.getRandomValueFromArray(raceTrackNumbers)});

                client.socket.on(PLAYER_ACTIONS, async (data) => {
                    const playerId = client.playerVO.id;
                    let playerActions = new PlayerActionsVO(data);

                    debug(`PLAYER_ACTIONS: `, data);

                    //preventing boosted players from boosting again
                    if (this._checkIfPlayerBoosted(playerId)){
                        playerActions.boost = false;
                    }

                    //and memorizing those who just boosted
                    if (playerActions.boost){
                        this._addBoostedPlayerId(playerId);
                    }

                    //saving player actions
                    this._setActionsByPlayerId(playerId, playerActions);

                    const counter = this._increaseReceivedActionsCounter();
                    if (counter == this.room.maxClients){
                        let dataToCompute = this._getDataToCompute();
                        //debug(`dataToCompute: `, dataToCompute);
                        const computedResults = await ComputingService(dataToCompute);
                        //debug(`computedResults: `, computedResults);
                        this._updatePlayersPositions(computedResults);
                        this._resetRoundData();
                        const roundResultsData = this._getRoundResultsData(computedResults);
                        this._broadcastEventToClients(ROUND_RESULTS, roundResultsData);
                    }
                })
            }
        }

        start(){
            debug(`START`)
            let initialDataByPlayerId = {};
            for (let playerId of this.room.playersIds){
                const position = this._getPositionByPlayerId(playerId);
                initialDataByPlayerId[playerId] = {playerId, milestones: [position]};
            }
            const initialData = this._getRoundResultsData(initialDataByPlayerId);
            //debug(`START_GAME with initialData: `, initialData)
            this._broadcastEventToClients(START_GAME, initialData);
        }

        _getRoundResultsData(computedResults){
            //creating a final data structure for clients
            const data = {results:[]};
            //and filling the data with final results per player
            for (let playerId in computedResults) if (hasOwnProperty.call(computedResults, playerId)){
                const playerResults = computedResults[playerId];
                //if there is no place prop in player's results data meaning the player has not finished the race yet
                if (!playerResults.place){
                    //we need to decorate the data with new random distance property for next round
                    const newRandomDistance = this._getRandomDistanceByPlayerId(playerId);
                    //and take a penalty from it if the player had boosted before
                    let boostPenalty = (this._checkIfPlayerBoosted(playerId)) ? 1 : 0;
                    const distanceAfterBoostPenalty = newRandomDistance - boostPenalty;
                    playerResults.distance = distanceAfterBoostPenalty;
                    this._setDistanceByPlayerId(playerId, playerResults.distance);
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

        _updatePlayersPositions(computedResults){
            for (let playerId in computedResults) if (hasOwnProperty.call(computedResults, playerId)) {
                const milestones = computedResults[playerId].milestones;
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

        _getBoostedPlayerIds(){
            const result = _boostedPlayerIds.get(this);
            return result;
        }

        _addBoostedPlayerId(id){
            const ids = this._getBoostedPlayerIds();
            ids[id] = id;
            _boostedPlayerIds.set(this, ids);
        }

        _checkIfPlayerBoosted(playerId){
            let result = (this._getBoostedPlayerIds()[playerId] !== undefined);
            return result;
        }
    }

    return GameplayController;
})()