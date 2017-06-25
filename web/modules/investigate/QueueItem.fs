namespace AntichessCheatDetection.Modules.Investigate

open MongoDB.Bson.Serialization.Attributes

[<AllowNullLiteral>]
type QueueItem() =
    member val Id : MongoDB.Bson.ObjectId = MongoDB.Bson.ObjectId.Empty with get, set
    [<BsonElement("name")>]
    member val Name : string = null with get, set
    [<BsonElement("requested")>]
    member val Requested : System.DateTime = System.DateTime.MinValue with get, set
    [<BsonElement("status")>]
    member val Status : QueueItemStatus = QueueItemStatus.Unprocessed with get, set