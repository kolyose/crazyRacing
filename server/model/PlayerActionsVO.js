'use strict';

export default (function() {
  let _direction = new WeakMap();
  let _boost = new WeakMap();
  let _superpower = new WeakMap();

  class PlayerActionsVO {
    constructor({ direction, boost, superpower }) {
      this.direction = direction;
      this.boost = boost;
      this.superpower = superpower;
    }

    set direction(value) {
      _direction.set(this, value);
    }

    get direction() {
      return _direction.get(this);
    }

    set boost(value) {
      _boost.set(this, value);
    }

    get boost() {
      return _boost.get(this);
    }

    set superpower(value) {
      _superpower.set(this, value);
    }

    get superpower() {
      return _superpower.get(this);
    }

    toString() {
      return {
        direction: this.direction,
        boost: this.boost,
        superpower: this.superpower,
      }.toString();
    }
  }

  return PlayerActionsVO;
})();
