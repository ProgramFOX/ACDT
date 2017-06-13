namespace AntichessCheatDetection.Modules.Reference

open MongoDB.Bson.Serialization.Attributes
open MongoDB.Bson

open System.Collections.Generic

type Reference() =
    [<BsonElement("max")>]
    member val Max : float = 0.0 with get, set

    [<BsonElement("sjeng_full")>]
    member val SjengFull : List<int> = null with get, set

    [<BsonElement("sjeng")>]
    member val Sjeng : float = 0.0 with get, set

    [<BsonElement("stockfish_full")>]
    member val StockfishFull : List<int> = null with get, set

    [<BsonElement("stockfish")>]
    member val Stockfish : float = 0.0 with get, set

    [<BsonElement("rating")>]
    member val Rating : int = 0 with get, set

    [<BsonElement("legit")>]
    member val Legit : bool = true with get, set

    [<BsonElement("speed")>]
    member val Speed : string = null with get, set

    [<BsonElement("player")>]
    member val Player : string = null with get, set

    [<BsonElement("game_id")>]
    member val GameId : string = null with get, set