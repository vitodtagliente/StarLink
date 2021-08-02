"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const WS = require("ws");
const encoding_1 = require("./encoding");
const starmessage_1 = require("./starmessage");
const ws = new WS('ws://localhost:7000');
ws.on('open', function open() {
    const message = new starmessage_1.default();
    message.Body = "something cool";
    ws.send(encoding_1.default.stringify(message));
});
ws.on('message', (data) => {
    const message = encoding_1.default.parse(data);
    console.log('received: %s', message.Body);
});
//# sourceMappingURL=index.js.map