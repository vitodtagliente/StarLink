export default class StarId
{
    public Id: string;

    public static Next(): StarId
    {
        const id: StarId = new StarId();
        var magic: number = Math.random() * (new Date()).getTime();
        id.Id = magic.toString(36).substr(2, 9);
        return id;
    }
}