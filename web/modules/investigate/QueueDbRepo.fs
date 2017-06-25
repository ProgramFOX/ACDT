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

type QueueDbRepo(settings: IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<QueueItem>(extractedSettings.QueueCollectionName)

    interface IQueueDbRepo with
        member this.GetNextQueueItem() =
            collection.Find(Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Unprocessed)).Sort(Builders<QueueItem>.Sort.Ascending(new StringFieldDefinition<QueueItem>("requested"))).FirstOrDefault()
        
        member this.SetInProgress (playerName : string) =
            collection.UpdateMany(
                Builders<QueueItem>.Filter.Eq(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.Unprocessed),
                Builders<QueueItem>.Update.Set(new StringFieldDefinition<QueueItem, QueueItemStatus>("status"), QueueItemStatus.InProgress)
            ).IsAcknowledged