import {MILESTONE_TYPE_MOVEMENT, MILESTONE_TYPE_BOOST, MILESTONE_TYPE_BLOCKED} from './BaseMilestoneVO';
import BaseMilestoneVO from './BaseMilestoneVO';
import MovementMilestoneVO from './MovementMilestoneVO';
import BoostMilestoneVO from './BoostMilestoneVO';
import BlockedMilestoneVO from './BlockedMilestoneVO';

class MilestonesFactory{

    getMilestoneByType(type, ...args){
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

        milestone.setData(...args);
        return milestone;
    }
}

export default new MilestonesFactory();