import Encoding from "./encoding";
import StarId from "./starid";

enum Version
{
    v1 = 1.0
}

export enum MessageHeaderField
{
    Version = 'version',
    Command = 'command',
    CommandPhase = 'command_phase'
}

export class MessageHeader
{
    public Id: StarId;
    public Fields: Map<string, string>;

    public constructor()
    {
        this.Id = StarId.Next()
        this.Fields = new Map<string, string>();
        this.Fields.set(MessageHeaderField.Version, Version.v1.toString());
    }
}

export type MessageBody = string;

export default class StarMessage
{
    public Header: MessageHeader;
    public Body: MessageBody;

    public constructor()
    {
        this.Header = new MessageHeader;
    }

    public static parse(data: string): StarMessage
    {
        try
        {
            return Encoding.parse<StarMessage>(data);
        }
        catch
        {
            console.warn(`Failed to parse the message[${data}]`);
            return null;
        }
    }
}