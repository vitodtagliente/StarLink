"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MessageHeader = exports.MessageHeaderField = void 0;
const encoding_1 = require("./encoding");
const starid_1 = require("./starid");
var Version;
(function (Version) {
    Version[Version["v1"] = 1] = "v1";
})(Version || (Version = {}));
var MessageHeaderField;
(function (MessageHeaderField) {
    MessageHeaderField["Version"] = "version";
    MessageHeaderField["Command"] = "command";
    MessageHeaderField["CommandPhase"] = "command_phase";
})(MessageHeaderField = exports.MessageHeaderField || (exports.MessageHeaderField = {}));
class MessageHeader {
    constructor() {
        this.Id = starid_1.default.Next();
        this.Fields = new Map();
        this.Fields.set(MessageHeaderField.Version, Version.v1.toString());
    }
}
exports.MessageHeader = MessageHeader;
class StarMessage {
    constructor() {
        this.Header = new MessageHeader;
    }
    static parse(data) {
        try {
            return encoding_1.default.parse(data);
        }
        catch (_a) {
            console.warn(`Failed to parse the message[${data}]`);
            return null;
        }
    }
}
exports.default = StarMessage;
//# sourceMappingURL=starmessage.js.map