namespace AntichessCheatDetection.Modules.Investigate

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System
open System.Collections.Generic

type IInvestigateDbRepo =
    abstract member GetGamesByPlayer: string -> List<Investigation>
    abstract member InsertInvestigations: seq<Investigation> -> unit

type InvestigateDbRepo(settings : IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<Investigation>(extractedSettings.InvestigateCollectionName)

    let gameExistsInDb gameId playerName =
        collection.Find(Builders<Investigation>.Filter.Eq(StringFieldDefinition<Investigation, string>("game_id"), gameId)
        &&& Builders<Investigation>.Filter.Eq(StringFieldDefinition<Investigation, string>("player"), playerName)).Any()


    interface IInvestigateDbRepo with
        
        member this.GetGamesByPlayer player =
            collection.Find(Builders<Investigation>.Filter.Eq(StringFieldDefinition<Investigation, string>("player"), player))
                .Project<Investigation>(BsonDocumentProjectionDefinition<Investigation, Investigation>(BsonDocument.Parse("{ _id: 0 }"))).ToList()
        
        member this.InsertInvestigations (investigations : seq<Investigation>) =
            for x in investigations do
                if not (gameExistsInDb x.GameId x.Player) then collection.InsertOne x