namespace AntichessCheatDetection.Modules.Investigate

[<AllowNullLiteral>]
type QueueItem() =
    member val Id : MongoDB.Bson.ObjectId = MongoDB.Bson.ObjectId.Empty with get, set
    member val Name : string = null with get, set
    member val Requested : System.DateTime = System.DateTime.MinValue with get, set
    member val Status : QueueItemStatus = QueueItemStatus.Unprocessed with get, set