namespace AntichessCheatDetection.Modules.Investigate

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System
open System.Collections.Generic

type IQueueDbRepo =
    abstract member GetNextQueueItem : unit -> QueueItem

type QueueDbRepo(settings: IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<QueueItem>(extractedSettings.QueueCollectionName)

    interface IQueueDbRepo with
        member this.GetNextQueueItem() =
            collection.Find(BsonDocumentFilterDefinition<QueueItem>(BsonDocument())).Sort(Builders<QueueItem>.Sort.Ascending(new StringFieldDefinition<QueueItem>("requested"))).FirstOrDefault()