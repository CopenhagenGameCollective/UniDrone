// tools.js
// ========
module.exports = {
  clamp:  function(a, b, c) {
    return Math.max(b, Math.min(c, a));
  },
  clamp01: function(a) {
    return Math.max(0.0, Math.min(1.0, a));
  }
};
