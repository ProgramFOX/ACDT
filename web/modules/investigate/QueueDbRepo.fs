namespace AntichessCheatDetection.Modules.Investigate

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System
open System.Collections.Generic

type IQueueDbRepo =
    abstract member GetNextQueueItem : unit -> QueueItem
    abstract member SetInProgress : string -> bool
    abstract member SetAsProcessed : string -> bool
    abstract member GetUnprocessedNames : unit -> seq<string>
    abstract member IsInQueueAndNotProcessed: string -> bool
    abstract member AddToQueue : string -> unit

type QueueDbRepo(settings: IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<QueueItem>(extractedSettings.QueueCollectionName)

    interface IQueueDbRepo with
        member this.GetNextQueueItem() =
            collection.Find(Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Unprocessed)).Sort(Builders<QueueItem>.Sort.Ascending(new StringFieldDefinition<QueueItem>("requested"))).FirstOrDefault()
        
        member this.SetInProgress (playerName : string) =
            collection.UpdateMany(
                Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Unprocessed) &&& Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, string>("name"), playerName),
                Builders<QueueItem>.Update.Set(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.InProgress)
            ).IsAcknowledged
        
        member this.SetAsProcessed (playerName : string) =
            collection.UpdateMany(
                Builders<QueueItem>.Filter.Ne(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Processed) &&& Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, string>("name"), playerName),
                Builders<QueueItem>.Update.Set(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Processed)
            ).IsAcknowledged
        
        member this.GetUnprocessedNames() =
            let unprocessedQueueItems = collection.Find(Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Unprocessed)).ToList()
            Seq.map (fun (x : QueueItem) -> x.Name) unprocessedQueueItems
        
        member this.IsInQueueAndNotProcessed (name : string) =
            collection.Find(Builders<QueueItem>.Filter.Ne(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Processed) &&& Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, string>("name"), name)).Any()
        
        member this.AddToQueue (name : string) =
            let item = QueueItem()
            item.Id <- ObjectId.GenerateNewId()
            item.Name <- name
            item.Status <- QueueItemStatus.Unprocessed
            item.Requested <- DateTime.UtcNow
            collection.InsertOne(item)