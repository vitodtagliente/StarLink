import * as WS from 'ws';
import Encoding from './encoding';
import StarMessage from './starmessage';

const ws = new WS('ws://localhost:7000');

ws.on('open', function open()
{
    const message: StarMessage = new StarMessage();
    message.Body = "something cool";
    ws.send(Encoding.stringify(message));
});

ws.on('message', (data: string) =>
{
    const message: StarMessage = Encoding.parse<StarMessage>(data);
    console.log('received: %s', message.Body);
});