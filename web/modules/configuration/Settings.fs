namespace AntichessCheatDetection.Modules.Configuration

type Settings() =
    member val MongoConnectionString : string = null with get, set
    member val Database : string = null with get, set
    member val ReferenceCollectionName : string = null with get, set
    member val InvestigateCollectionName : string = null with get, set