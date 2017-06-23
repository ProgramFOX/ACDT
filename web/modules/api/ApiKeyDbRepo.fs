namespace AntichessCheatDetection.Modules.Api

open AntichessCheatDetection.Modules.Configuration

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Options
open System
open System.Security.Cryptography
open System.Text

type IApiKeyDbRepo =
    abstract member IsValidKey: string -> bool

type ApiKeyDbRepo(settings: IOptions<Settings>) =
    let extractedSettings = settings.Value
    let collection = MongoClient(extractedSettings.MongoConnectionString : string).GetDatabase(extractedSettings.Database).GetCollection<BsonDocument>(extractedSettings.ApiKeyCollectionName)

    interface IApiKeyDbRepo with
        member this.IsValidKey unhashedKey =
            let bytes = Encoding.ASCII.GetBytes unhashedKey
            let sha1 = SHA1.Create()
            let hash = sha1.ComputeHash bytes
            let hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower()
            collection.Find(Builders<BsonDocument>.Filter.Eq(StringFieldDefinition<BsonDocument, string>("hash"), hashHex)).Any()