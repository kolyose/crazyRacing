import {FIELD_WIDTH, FIELD_LENGTH} from './../model/constants'
import Debug from './../debug'
const debug = new Debug('CR:ComputingService')

export default function (data){
        return new Promise((resolve, reject) => {

            const STEP = 1;
            const BOOST = 2;
            const hasOwnProperty = Object.prototype.hasOwnProperty;
            let milestonesByPlayerId = {};

            //initialization of game field which is a two-dimensional array representing all possible positions for players
            let slotsPerRacetrack = [];
            for (let i=0; i<FIELD_WIDTH; i++){
                let racetrackSlots=[];
                for (let j=0; j<FIELD_LENGTH;j++){
                    racetrackSlots.push(null);
                }
                slotsPerRacetrack.push(racetrackSlots);
            }

            //setting players onto their positions on the field
            // and initializing in the same loop for optimization purpose
            for (let playerId in data) if (hasOwnProperty.call(data, playerId)){
                slotsPerRacetrack[data[playerId].position.y][data[playerId].position.x] = playerId;
                milestonesByPlayerId[playerId] = [];
            }

            //debug(`getRemainingDistanceSum(data): ${getRemainingDistanceSum(data)}`)
            while (getRemainingDistanceSum(data) > 0){
                //1st STAGE:
                //we need to grab only BOOSTing players by priority: from the forward player of top racetrack
                for (let racetrackIndex=0; racetrackIndex<FIELD_WIDTH; racetrackIndex++) {
                    for (let slotIndex = FIELD_LENGTH - 1; slotIndex > -1; slotIndex--) {
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
                 for (let racetrackIndex=0; racetrackIndex<FIELD_WIDTH; racetrackIndex++) {
                    for (let slotIndex = FIELD_LENGTH - 1; slotIndex > -1; slotIndex--) {
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

            //debug(`milestonesByPlayerId:`, milestonesByPlayerId)
            //after milestones data filled we need to optimize it by removing identical milestones
            for (let playerId in milestonesByPlayerId) if (hasOwnProperty.call(milestonesByPlayerId, playerId)){
               // debug(`playerId: ${playerId}`)
               // debug(`oldMilestones:`,milestonesByPlayerId[playerId])
                let oldMilestones = milestonesByPlayerId[playerId];
                let newMilestones = [];
                for (let i=0; ; i++){
                    //if it's a last milestone we need to pick it
                    //but only if it wasn't picked earlier as a shift-related one
                    if (i === oldMilestones.length-1){
                        if (!newMilestones.length || oldMilestones[i].y === newMilestones[newMilestones.length-1].y){
                            newMilestones.push(oldMilestones[i]);
                        }
                        break;
                    }
                    //if milestones are on different racetracks it means there was shifting
                    //so we need both to be picked
                    if (oldMilestones[i].y !==  oldMilestones[i+1].y){
                        newMilestones.push(oldMilestones[i]);
                        newMilestones.push(oldMilestones[i+1]);
                    }
                }
                milestonesByPlayerId[playerId] = newMilestones;
            }

            resolve(milestonesByPlayerId);

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

                    milestonesByPlayerId[playerId].push({
                            x: slotIndex,
                            y:racetrackIndex,
                            s:(data[playerId].actions.boost ? 2 : 1)
                        });

                    milestonesByPlayerId[playerId].push({
                            x: slotIndex,
                            y:targetRacetrackIndex,
                            s:(data[playerId].actions.boost ? 2 : 1)
                        });

                     //do not forget to reset shifting request to avoid double shifting
                    data[playerId].actions.direction = 0;
                }
                else
                {
                    //in other case move player by 1 step forward if nobody other takes that position
                    let targetSlotIndex = slotIndex + 1;
                    if (!slotsPerRacetrack[racetrackIndex][targetSlotIndex]){
                        slotsPerRacetrack[racetrackIndex][slotIndex] = null;
                        slotsPerRacetrack[racetrackIndex][targetSlotIndex] = playerId;

                        milestonesByPlayerId[playerId].push({
                                x: targetSlotIndex,
                                y:racetrackIndex,
                                s:(data[playerId].actions.boost ? 2 : 1)
                            });
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