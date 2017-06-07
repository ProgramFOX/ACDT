namespace AntichessCheatDetection.Modules.Reference

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System

type IReferenceDbRepo =
    abstract member GetDistribution: string -> string -> int -> int -> int -> string -> list<int64>

type ReferenceDbRepo(settings : IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<Reference>(extractedSettings.ReferenceCollectionName)

    let countClosestTo legit engine center dist minRating maxRating speed =
        let filter = BsonDocumentFilterDefinition(
                         BsonDocument.Parse(
                             String.Format("{{ legit: {0}, {1}: {{ $gte: {2}, $lt: {3} }}, rating: {{ $gte: {4}, $lte: {5} }} {6} }}", (if legit then "true" else "false"), engine, (center - dist) / 100.0, (center + dist) / 100.0, minRating, maxRating, (if speed.Equals("all") then "" else ", speed: \"" + speed + "\""))
                         ))
        collection.Find(filter).Count()


    interface IReferenceDbRepo with
        member this.GetDistribution legitOrCheat engine step minRating maxRating speed =
            [ for i in 0 .. step .. 100 -> countClosestTo (legitOrCheat.Equals "legit") engine (double i) (double step / 2.0) minRating maxRating speed]