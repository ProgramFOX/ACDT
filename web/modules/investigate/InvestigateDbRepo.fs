namespace AntichessCheatDetection.Modules.Investigate

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System
open System.Collections.Generic

type IReferenceDbRepo =
    abstract member GetGamesByPlayer: string -> List<Investigation>

type ReferenceDbRepo(settings : IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<Investigation>(extractedSettings.InvestigateCollectionName)


    interface IReferenceDbRepo with
        
        member this.GetGamesByPlayer player =
            collection.Find(Builders<Investigation>.Filter.Eq(StringFieldDefinition<Investigation, string>("player"), player))
                .Project<Investigation>(BsonDocumentProjectionDefinition<Investigation, Investigation>(BsonDocument.Parse("{ _id: 0 }"))).ToList()