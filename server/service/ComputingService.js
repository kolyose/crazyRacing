import {FIELD_WIDTH, FIELD_LENGTH, STEP_SPEED, BOOST_SPEED} from './../model/constants';
import {MILESTONE_TYPE_MOVEMENT, MILESTONE_TYPE_BOOST} from './model/BaseMilestoneVO';
import milestonesFactory from './model/MilestonesFactory';
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
                        if (!playerId || data[playerId].distance == 0 || !data[playerId].actions.boost) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);
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
                        if (!playerId || data[playerId].distance == 0) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);
                    }
                }
            }

            //after milestones data filled we need to optimize it by removing identical milestones
            for (let playerId in milestonesByPlayerId) if (hasOwnProperty.call(milestonesByPlayerId, playerId)){
               // debug(`playerId: ${playerId}`)
               // debug(`rawMilestones:`,milestonesByPlayerId[playerId])
                let rawMilestones = milestonesByPlayerId[playerId];
                let optimizedMilestones = [];
                for (let i=0; ; i++){
                    //if it's a last milestone we need to pick it
                    //but only if it wasn't picked earlier as a shift-related one
                    if (i === rawMilestones.length-1){
                        if (!optimizedMilestones.length || rawMilestones[i].y === optimizedMilestones[optimizedMilestones.length-1].y){
                            optimizedMilestones.push(rawMilestones[i]);
                        }
                        break;
                    }
                    
                    //if current milestone is not mevement-based one - we cannot to remove it
                    if (rawMilestones[i].type !== MILESTONE_TYPE_MOVEMENT && rawMilestones[i].type !== MILESTONE_TYPE_BOOST){
                        optimizedMilestones.push(rawMilestones[i]);
                    }

                    //if milestones are on different racetracks it means there was shifting
                    //so we need both to be picked
                    if (rawMilestones[i].y !==  rawMilestones[i+1].y){
                        optimizedMilestones.push(rawMilestones[i]);
                        optimizedMilestones.push(rawMilestones[i+1]);
                    }
                }
               
                Array.prototype.map.call(optimizedMilestones, (milestone) => {
                    return milestone.getData();
                });
                results[playerId].milestones = optimizedMilestones;                
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
                const targetRacetrackIndex = racetrackIndex + data[playerId].actions.direction;

                if (data[playerId].actions.direction !== 0
                    && slotsPerRacetrack[targetRacetrackIndex] !== undefined
                    && slotsPerRacetrack[targetRacetrackIndex][slotIndex] === null)
                {
                    //then we can shift the player to the position
                    slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                    slotsPerRacetrack[targetRacetrackIndex][slotIndex] = playerId;

                    // initializing milestone data
                    let currentMilestone = (data[playerId].actions.boost) ? 
                        milestonesFactory.getBoostMilestone(slotIndex,racetrackIndex,BOOST_SPEED) :
                        milestonesFactory.getMovementMilestone(slotIndex, racetrackIndex);

                    //save current position
                    milestonesByPlayerId[playerId].push(currentMilestone);
                   
                    //save next position
                    const nextMilestone = milestonesFactory.cloneMilestone(currentMilestone);
                    nextMilestone.y = targetRacetrackIndex;
                    milestonesByPlayerId[playerId].push(nextMilestone);

                    //do not forget to reset shifting request to avoid double shifting
                    data[playerId].actions.direction = 0;
                }
                else
                {
                    //in other case move player by 1 step forward
                    let targetSlotIndex = slotIndex + 1;

                     // initializing milestone data
                    let currentMilestone = (data[playerId].actions.boost) ? 
                        milestonesFactory.getBoostMilestone(targetSlotIndex,racetrackIndex,BOOST_SPEED) :
                        milestonesFactory.getMovementMilestone(targetSlotIndex, racetrackIndex);

                    //if a player has reached the finish line we need to save his place taken
                    //and exclude him from further calculations
                    if (targetSlotIndex >= FIELD_LENGTH){
                        finishers.push(playerId);
                        data[playerId].distance = 0;
                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                        milestonesByPlayerId[playerId].push(currentMilestone);

                        return;
                    }

                    // in other case we can move the player forward
                    // if nobody other takes target position
                    if (!slotsPerRacetrack[racetrackIndex][targetSlotIndex]){
                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                        slotsPerRacetrack[racetrackIndex][targetSlotIndex] = playerId;
                        milestonesByPlayerId[playerId].push(currentMilestone);
                    } else {
                        //or just notify player he is blocked
                        currentMilestone = milestonesFactory.getBlockedMilestone();
                        milestonesByPlayerId[playerId].push(currentMilestone);
                    }
                }

                // even if there were no shifting or moving done
                // we still need to update the player's remaining distance
                data[playerId].distance = (data[playerId].distance - 1);
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