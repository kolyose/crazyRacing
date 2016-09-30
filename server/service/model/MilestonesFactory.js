import {MILESTONE_TYPE_MOVEMENT, MILESTONE_TYPE_BOOST, MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import BaseMilestoneVO from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import BoostMilestoneVO from './BoostMilestoneVO';
import BlockedMilestoneVO from './BlockedMilestoneVO';

class MilestonesFactory{

    getMilestoneByType(type){
        let milestone = new BaseMilestoneVO();
        switch (type){
            case MILESTONE_TYPE_MOVEMENT: {
                milestone = new MovementMilestoneVO();
                break;
            }
            case MILESTONE_TYPE_BOOST: {
                milestone = new BoostMilestoneVO();
                break;
            }
            case MILESTONE_TYPE_BLOCKED: {
                milestone = new BlockedMilestoneVO();
                break;
            }

            default:
                break;
        }
        return milestone;
    }

    cloneMilestone(originMilestone){
        let newMilestone = this.getMilestoneByType(originMilestone.type);
        newMilestone.setData(originMilestone.getData());
        return newMilestone;
    }

    getMovementMilestone(x, y){
        const milestone = this.getMilestoneByType(MILESTONE_TYPE_MOVEMENT);
        milestone.x = x;
        milestone.y = y;
        return milestone;
    }

     getBoostMilestone(x, y, speed){
        const milestone = this.getMilestoneByType(MILESTONE_TYPE_BOOST);
        milestone.x = x;
        milestone.y = y;
        milestone.speed = speed;
        return milestone;
    }

     getBlockedMilestone(){
        const milestone = this.getMilestoneByType(MILESTONE_TYPE_BLOCKED);       
        return milestone;
    }
}

export default new MilestonesFactory();