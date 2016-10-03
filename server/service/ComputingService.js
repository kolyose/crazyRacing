import {FIELD_WIDTH, FIELD_LENGTH, STEP_SPEED, BOOST_SPEED, BLOCKED_SPEED} from './../model/constants'
import Debug from './../debug'
const debug = new Debug('CR:ComputingService')

export default function (data){
        return new Promise((resolve, reject) => {

            const hasOwnProperty = Object.prototype.hasOwnProperty;
            let results = {};
            let milestonesByPlayerId = {};
            let finishers = [];

            //initialization of game field which is a two-dimensional array representing all possible positions for players
            let slotsPerRacetrack = [];
            for (let i=0; i<FIELD_WIDTH; i++){
                let racetrackSlots=[];
                for (let j=0; j<FIELD_LENGTH;j++){
                    racetrackSlots.push(null);
                }
                slotsPerRacetrack.push(racetrackSlots);
            }

            //performing various operations with input data withing single loop for optimization purposes:
            for (let playerId in data) if (hasOwnProperty.call(data, playerId)){
                //setting players onto their positions on the field
                slotsPerRacetrack[data[playerId].position.y][data[playerId].position.x] = playerId;

                //initializing temp data
                milestonesByPlayerId[playerId] = [];

                //initializing output data
                results[playerId] = {playerId};
            }

            // calculating milestones until all players complete their distances
            while (getRemainingDistanceSum(data) > 0){
                //1st STAGE:
                //we need to grab only BOOSTing players by priority: from the forward player of top racetrack
                for (let slotIndex = FIELD_LENGTH-1; slotIndex > -1; slotIndex--) {
                    for (let racetrackIndex=FIELD_WIDTH-1; racetrackIndex > -1; racetrackIndex--) {
                        let playerId = slotsPerRacetrack[racetrackIndex][slotIndex];
                        //if there is no player at current slot
                        //or the player doesn't have more distance to move
                        //or he didn't request boost
                        //then check next slot
                         
                         if (playerId){
                            debug(`playerId ${playerId} FIRST ITERATION`);
                             debug(`distance ${data[playerId].distance} `);
                            debug(`boost ${data[playerId].actions.boost} `);
                        }
                        if (!playerId || data[playerId].distance == 0 || !data[playerId].actions.boost) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward                       
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);
                        debug(`///////////////////////////////////////////////////`);
                    }
                }

                //2nd STAGE:
                //now we need to grab all players (including previously boosted ones) by the same priority
                for (let slotIndex = FIELD_LENGTH - 1; slotIndex > -1; slotIndex--) {
                    for (let racetrackIndex=FIELD_WIDTH-1; racetrackIndex > -1; racetrackIndex--) {
                        let playerId = slotsPerRacetrack[racetrackIndex][slotIndex];
                        //if there is no player at current slot
                        //or the player doesn't have more distance to move
                        //then check next slot
                         if (playerId){
                         debug(`playerId ${playerId} SECOND ITERATION`);
                         debug(`distance ${data[playerId].distance} `);
                         }
                        if (!playerId || data[playerId].distance == 0) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward                       
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);                        

                        // even if there were no shifting or moving done
                        // we still need to update the player's remaining distance
                        data[playerId].distance = (data[playerId].distance - 1);
                        debug(`///////////////////////////////////////////////////`);
                    }
                }
            }

            //after milestones data filled we need to optimize it by removing identical milestones
            for (let playerId in milestonesByPlayerId) if (hasOwnProperty.call(milestonesByPlayerId, playerId)){
                let rawMilestones = milestonesByPlayerId[playerId];
                let optimizedMilestones = optimizeMilestones(rawMilestones);
                results[playerId].milestones = rawMilestones;
            }          

            //after all milestones-related calculations done we need to check if there are finishers
            //and if so we need to set places for all unfinished players to make game completed
            if (finishers.length > 0) {
                for (let slotIndex = FIELD_LENGTH - 1; slotIndex > -1; slotIndex--) {
                    for (let racetrackIndex = FIELD_WIDTH - 1; racetrackIndex > -1; racetrackIndex--) {
                        let playerId = slotsPerRacetrack[racetrackIndex][slotIndex];
                        if (!playerId) continue;
                        finishers.push(playerId);
                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                    }
                }

                //setting place prop for all players depending on their finishing order
                for (let i=0; i<finishers.length; i++){
                    results[finishers[i]].place = i+1;
                }
            }

            resolve(results);

            function shiftOrMovePlayer(playerId, racetrackIndex, slotIndex){
                //if the player had requested shifting
                //and nobody other takes the neighbour position

                debug(`playerId ${playerId} old pos: ${racetrackIndex} ${slotIndex}`)

                const targetRacetrackIndex = racetrackIndex + data[playerId].actions.direction;

                if (data[playerId].actions.direction !== 0
                    && slotsPerRacetrack[targetRacetrackIndex] !== undefined
                        && slotsPerRacetrack[targetRacetrackIndex][slotIndex] === null)
                {
                    //then we can shift the player to the position
                    slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                    slotsPerRacetrack[targetRacetrackIndex][slotIndex] = playerId;

                    debug(`playerId ${playerId} new pos: ${targetRacetrackIndex} ${slotIndex}`)

                    milestonesByPlayerId[playerId].push({
                            x: slotIndex,
                            y:racetrackIndex,
                            s:(data[playerId].actions.boost ? BOOST_SPEED : STEP_SPEED)
                        });

                    milestonesByPlayerId[playerId].push({
                            x: slotIndex,
                            y:targetRacetrackIndex,
                            s:(data[playerId].actions.boost ? BOOST_SPEED : STEP_SPEED)
                        });

                     //do not forget to reset shifting request to avoid double shifting
                    data[playerId].actions.direction = 0;
                }
                else
                {
                    //in other case move player by 1 step forward
                    let targetSlotIndex = slotIndex + 1;

                    //if a player has reached the finish line we need to save his place taken
                    //and exclude him from further calculations
                    if (targetSlotIndex >= FIELD_LENGTH){
                        finishers.push(playerId);
                        data[playerId].distance = 0;
                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                        milestonesByPlayerId[playerId].push({
                            x: targetSlotIndex,
                            y: racetrackIndex,
                            s:(data[playerId].actions.boost ? BOOST_SPEED : STEP_SPEED)
                        });

                        return;
                    }
              
                    // otherwise we can move the player forward
                    // if nobody other takes target position
                    if (!slotsPerRacetrack[racetrackIndex][targetSlotIndex]){

                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                        slotsPerRacetrack[racetrackIndex][targetSlotIndex] = playerId;

                         debug(`playerId ${playerId} new pos: ${racetrackIndex} ${targetSlotIndex}`)

                        milestonesByPlayerId[playerId].push({
                                x: targetSlotIndex,
                                y:racetrackIndex,
                                s:(data[playerId].actions.boost ? BOOST_SPEED : STEP_SPEED)
                            });
                    } else { // or leave the player at his place with speed decrease in case if the next position is blocked by someone else                        

                        debug(`playerId ${playerId} new pos: ${racetrackIndex} ${slotIndex}`)

                         milestonesByPlayerId[playerId].push({
                                x:slotIndex,
                                y:racetrackIndex,
                                s:BLOCKED_SPEED
                            });
                    }
                }
            }

            //TODO: REFACTOR the method in order to make it simpler, with only one round (loop) of optimizations
            function optimizeMilestones(rawMilestones){
                if (!rawMilestones.length) return rawMilestones;

                let firstRoundResult = [],
                    secondRoundResult = [];  
                for (let i=0; ; i++){
                    //if it's a last milestone we need to pick it
                    //but only if it wasn't picked earlier as a shift-related one
                    if (i === rawMilestones.length-1){
                        if (!firstRoundResult.length || rawMilestones[i].y === firstRoundResult[firstRoundResult.length-1].y){
                            firstRoundResult.push(rawMilestones[i]);
                        }
                        break;
                    }
                    //if milestones are on different racetracks it means there was shifting
                    //or if milestone has different speed factor it means the was speed change
                    //in both cases we need both the milestones to be picked
                    if ((rawMilestones[i].y !== rawMilestones[i+1].y) || (rawMilestones[i].s !== rawMilestones[i+1].s)){
                        firstRoundResult.push(rawMilestones[i]);
                        firstRoundResult.push(rawMilestones[i+1]);
                    }
                }

                //second round of optimizations: we need to exclude dublicates from previous one
                const length = firstRoundResult.length;
                secondRoundResult.push(firstRoundResult[0]);
                for (let i=1; i<length-1; i++){
                    if (firstRoundResult[i].y !== firstRoundResult[i-1].y || firstRoundResult[i].s !== firstRoundResult[i-1].s){
                        secondRoundResult.push(firstRoundResult[i]);
                    }
                }

                const firstRoundLastElement = firstRoundResult[length-1],
                    secondRoundLastElement = secondRoundResult[secondRoundResult.length-1];

                if (firstRoundLastElement.x !== secondRoundLastElement.x 
                    || firstRoundLastElement.y !== secondRoundLastElement.y 
                        || firstRoundLastElement.s !== secondRoundLastElement.s){
                            
                            secondRoundResult.push(firstRoundResult[length-1]);
                        }
                return secondRoundResult;
            }

            function getRemainingDistanceSum(data){
                let sum=0;
                for (let playerId in data) if (hasOwnProperty.call(data, playerId)){
                    sum = sum + data[playerId].distance;
                }
                return sum;
            }
        })
}