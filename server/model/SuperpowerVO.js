'use strict';

export default (function() {
  const _properties = new WeakMap();

  class SuperpowerVO {
    constructor(props) {
      this.setProperties(props);
    }

    setProperties(newProps) {
      const props = _properties.get(this) || {};
      for (let prop in newProps) {
        props[prop] = newProps[prop];
      }
      _properties.set(this, props);
    }

    get type() {
      return _properties.get(this).type;
    }
  }
})();
