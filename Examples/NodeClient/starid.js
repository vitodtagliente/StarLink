"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class StarId {
    static Next() {
        const id = new StarId();
        var magic = Math.random() * (new Date()).getTime();
        id.Id = magic.toString(36).substr(2, 9);
        return id;
    }
}
exports.default = StarId;
//# sourceMappingURL=starid.js.map