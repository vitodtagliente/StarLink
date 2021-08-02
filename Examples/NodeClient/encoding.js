"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var EncodingType;
(function (EncodingType) {
    EncodingType["ASCII"] = "ascii";
    EncodingType["Base64"] = "base64";
})(EncodingType || (EncodingType = {}));
class Encoding {
    static encode(data) {
        let buffer = Buffer.from(data);
        return buffer.toString(EncodingType.Base64);
    }
    static decode(data) {
        let buffer = Buffer.from(data, EncodingType.Base64);
        return buffer.toString(EncodingType.ASCII);
    }
    // https://stackoverflow.com/questions/29085197/how-do-you-json-stringify-an-es6-map
    static stringify(data) {
        function replacer(key, value) {
            if (value instanceof Map) {
                return {
                    dataType: 'Map',
                    value: Array.from(value.entries()), // or with spread: value: [...value]
                };
            }
            else {
                return value;
            }
        }
        // return JSON.stringify(data, replacer);
        return JSON.stringify(data);
    }
    // https://stackoverflow.com/questions/29085197/how-do-you-json-stringify-an-es6-map
    static parse(data) {
        function reviver(key, value) {
            if (typeof value === 'object' && value !== null) {
                if (value.dataType === 'Map') {
                    return new Map(value.value);
                }
            }
            return value;
        }
        // return JSON.parse(data, reviver);
        return JSON.parse(data);
    }
    static tryParse(data) {
        try {
            return Encoding.parse(data);
        }
        catch (_a) {
            console.warn(`Failed to parse the data[${data}]`);
            return null;
        }
    }
}
exports.default = Encoding;
//# sourceMappingURL=encoding.js.map