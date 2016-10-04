import {FIELD_WIDTH, FIELD_LENGTH, STEP_SPEED, BOOST_SPEED} from './../model/constants';
import {MILESTONE_TYPE_BLOCKED, MILESTONE_TYPE_MOVEMENT, MILESTONE_TYPE_BOOST} from './model/BaseMilestoneVO';
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

            //creating pool for players already processed in recent iteration to prevent repeated handling of the same player
            let processedPlayers = [];
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

                        //if player found had been already processed in recent iteration we need to skip him
                        if (~processedPlayers.indexOf(playerId)) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);
                        processedPlayers.push(playerId);
                    }
                }

                processedPlayers = [];

                //2nd STAGE:
                //now we need to grab all players (including previously boosted ones) by the same priority
                for (let slotIndex = FIELD_LENGTH - 1; slotIndex > -1; slotIndex--) {
                    for (let racetrackIndex=FIELD_WIDTH-1; racetrackIndex > -1; racetrackIndex--) {
                        let playerId = slotsPerRacetrack[racetrackIndex][slotIndex];
                        //if there is no player at current slot
                        //or the player doesn't have more distance to move
                        //then check next slot
                        if (!playerId || data[playerId].distance == 0) continue;

                        //if player found had been already processed in recent iteration we need to skip him
                        if (~processedPlayers.indexOf(playerId)) continue;

                        //In other case shift a player if he requested & allowed to shifting
                        //or move him 1 step forward
                        shiftOrMovePlayer(playerId, racetrackIndex, slotIndex);
                        processedPlayers.push(playerId);

                        // even if there were no shifting or moving done
                        // we still need to update the player's remaining distance
                        data[playerId].distance = (data[playerId].distance - 1);
                    }
                }
                processedPlayers = [];
            }

            //after milestones data filled we need to optimize it by removing identical milestones
            for (let playerId in milestonesByPlayerId) if (hasOwnProperty.call(milestonesByPlayerId, playerId)){
               // debug(`playerId: ${playerId}`)
               // debug(`rawMilestones:`,milestonesByPlayerId[playerId])
                const rawMilestones = milestonesByPlayerId[playerId];
                let optimizedMilestones = optimizeMilestones(rawMilestones);

                //extracting data for proper serialization
                optimizedMilestones = Array.prototype.map.call(optimizedMilestones, (milestone) => {
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
                const MILESTONE_TYPE = (data[playerId].actions.boost) ? MILESTONE_TYPE_BOOST : MILESTONE_TYPE_MOVEMENT;

                if (data[playerId].actions.direction !== 0
                    && slotsPerRacetrack[targetRacetrackIndex] !== undefined
                    && slotsPerRacetrack[targetRacetrackIndex][slotIndex] === null)
                {
                    //then we can shift the player to the position
                    slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                    slotsPerRacetrack[targetRacetrackIndex][slotIndex] = playerId;

                   // debug(`player ${playerId} SHIFTED: ${racetrackIndex} ${slotIndex} => ${targetRacetrackIndex} ${slotIndex}`);

                    //save current position
                    const currentMilestone = milestonesFactory.getMilestoneByType(MILESTONE_TYPE, slotIndex, racetrackIndex);
                    milestonesByPlayerId[playerId].push(currentMilestone);
                   
                    //save next position
                    const nextMilestone = milestonesFactory.getMilestoneByType(MILESTONE_TYPE, slotIndex, targetRacetrackIndex);
                    milestonesByPlayerId[playerId].push(nextMilestone);

                    //do not forget to reset shifting request to avoid double shifting
                    data[playerId].actions.direction = 0;
                }
                else
                {
                    //in other case move player by 1 step forward
                    let targetSlotIndex = slotIndex + 1;

                    // initializing milestone data
                    let currentMilestone = milestonesFactory.getMilestoneByType(MILESTONE_TYPE, targetSlotIndex,racetrackIndex);

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

                       // debug(`player ${playerId} MOVED: ${racetrackIndex} ${slotIndex} => ${racetrackIndex} ${targetSlotIndex}`);
                    } else {
                        //or just notify player he is blocked
                        currentMilestone = milestonesFactory.getMilestoneByType(MILESTONE_TYPE_BLOCKED, slotsPerRacetrack[racetrackIndex][targetSlotIndex]);
                       // debug(`player ${playerId} BLOCKED by ${slotsPerRacetrack[racetrackIndex][targetSlotIndex]} from ${racetrackIndex} ${targetSlotIndex}`);
                        milestonesByPlayerId[playerId].push(currentMilestone);
                    }
                }
            }

            function  optimizeMilestones(rawMilestones){
                const result = [];
                result.push(rawMilestones[0]);

                for (let i=1, length=rawMilestones.length; i<length; i++){
                    //we need to save recent milestone if it differs from previous one:
                    //either by type
                    if (rawMilestones[i].type !== rawMilestones[i-1].type){
                        result.push(rawMilestones[i]);
                        continue;
                    }
                    //or by racetrack position
                    if (rawMilestones[i].y !== rawMilestones[i-1].y){
                        result.push(rawMilestones[i]);
                        continue;
                    }
                    //or if a type of both milestones is BLOCKED but by different players
                    if (rawMilestones[i].type === MILESTONE_TYPE_BLOCKED
                        && rawMilestones[i].blockerId !== rawMilestones[i-1].blockerId) {
                        result.push(rawMilestones[i]);
                        continue;
                    }
                    //or if it's the last milestone and it has a slot position different from previous one
                    if ((i == length-1) && (rawMilestones[i].x !== rawMilestones[i-1].x)){
                        result.push(rawMilestones[i]);
                        continue;
                    }
                }
                return result;
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