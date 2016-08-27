import {FIELD_WIDTH, PLAYER_ACTIONS, START_GAME, ROUND_RESULTS} from './constants'
import ComputingService from './../service/ComputingService'
import PlayerActionsVO from './PlayerActionsVO'
import Util from './../util'
import EventEmitter from 'events'

export default (function(){

    const _room = new WeakMap()
    const _receivedActionsCounter = new WeakMap()
    const _actionsByPlayerId = new WeakMap()
    const _positionsByPlayerId = new WeakMap()
    const _distancesByPlayerId = new WeakMap()

    class GameplayController extends EventEmitter{
        constructor(room){
            super()
            _room.set(this, room)
            _positionsByPlayerId.set(this, {})
            this._resetRoundData()
        }

        get room(){
            return _room.get(this)
        }

        init(){
            //initializing array of racetrack numbers for further initialization of players' random positions
            let raceTrackNumbers = []
            for (let i=0; i<FIELD_WIDTH; i++){
                raceTrackNumbers.push(i)
            }

            for (client of this.room.clients){
                //setting random initial racetrack position for each player
                this._setPositionByPlayerId(client.playerVO.id, {x:0, y:Util.getRandomValueFromArray(raceTrackNumbers)})

                client.socket.on(PLAYER_ACTIONS, async (data) => {
                    this._setActionsByPlayerId(client.playerVO.id, new PlayerActionsVO(data))
                    if (this._increaseReceivedActionsCounter() === this.room.maxClients){
                        const milestonesByPlayerId = await ComputingService(this._getDataToCompute())
                        const  roundResultsData = this._getRoundResultsData(milestonesByPlayerId)
                        this._broadcastEventToClients(ROUND_RESULTS, roundResultsData)
                        this._resetRoundData()
                    }
                })
            }
        }

        start(){
            let milestonesByPlayerId = {}
            for (let playerId in this.room.playersIds){
                milestonesByPlayerId[playerId] = [this._getPositionByPlayerId(playerId)]
            }
            const initialData = this._getRoundResultsData(milestonesByPlayerId)
            this._broadcastEventToClients(START_GAME, initialData)
        }

        _getRoundResultsData(milestonesByPlayerId){
            const data = {results:[]}
            let resultPerPlayer
            for (let playerId in this.room.playersIds){
                resultPerPlayer = {
                    playerId,
                    distance: this._getRandomDistanceByPlayerId(playerId),
                    milestones: milestonesByPlayerId[playerId]
                }
                data.results.push(resultPerPlayer)
            }
            return data
        }

        _broadcastEventToClients(event, data){
            for (client of this.room.clients){
                client.socket.emit(event, {data})
            }
        }

        _resetRoundData(){
            _receivedActionsCounter.set(this, 0)
            _actionsByPlayerId.set(this, {})
            _distancesByPlayerId.set(this, {})
        }

        _increaseReceivedActionsCounter(){
            let counter = _receivedActionsCounter.get(this)
            counter++
            _receivedActionsCounter.set(this, counter)
            return counter;
        }

        _getDataToCompute(){
            let data = {}

            for (client of this.room.clients){
                const playerId = client.playerVO.id
                data[playerId] = {
                    position: this._getPositionByPlayerId(playerId),
                    actions: this._getActionsByPlayerId(playerId),
                    distance: this._getDistanceByPlayerId(playerId)
                }
            }
            return data
        }

        _getRandomDistanceByPlayerId(playerId){
            const playerPosition = this._getPositionByPlayerId(playerId)
            const randomDistance =  this._calculateRandomDistanceByPosition(playerPosition.y)
            this._setDistanceByPlayerId(playerId, randomDistance)
            return randomDistance
        }

        _calculateRandomDistanceByPosition(position){
            let randomDistance
            switch(position){
                case 0:{
                    randomDistance = Math.floor(Math.random()*4) + 1
                    break
                }

                case 1:{
                    randomDistance = Math.floor(Math.random()*3) + 2
                    break
                }

                case 2:{
                    randomDistance = Math.floor(Math.random()*4) + 2
                    break
                }

                case 3:{
                    randomDistance = Math.floor(Math.random()*3) + 3
                    break
                }

                case 4:{
                    randomDistance = Math.floor(Math.random()*4) + 3
                    break
                }

                case 5:{
                    randomDistance = Math.floor(Math.random()*3) + 4
                    break
                }
                    default: randomDistance = 0
            }

            return randomDistance
        }

        _getPositionByPlayerId(playerId){
            let positions = _positionsByPlayerId.get(this)
            return positions[playerId]
        }

        _setPositionByPlayerId(playerId, newPosition){
            let positions = _positionsByPlayerId.get(this)
            positions[playerId] = newPosition
            _positionsByPlayerId.set(this, positions)
        }

        _getActionsByPlayerId(playerId){
            let actions = _actionsByPlayerId.get(this)
            return actions[playerId]
        }

        _setActionsByPlayerId(playerId, newActions){
            let actions = _actionsByPlayerId.get(this)
            actions[playerId] = newActions
            _actionsByPlayerId.set(this, actions)
        }

        _getDistanceByPlayerId(playerId){
            let distances = _distancesByPlayerId.get(this)
            return distances[playerId]
        }

        _setDistanceByPlayerId(playerId, newDistance){
            let distances = _distancesByPlayerId.get(this)
            distances[playerId] = newDistance
            _distancesByPlayerId.set(this, distances)
        }
    }

    return GameplayController
})()