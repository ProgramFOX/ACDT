namespace AntichessCheatDetection.Modules.Reference

type Reference(_id, max, sjeng_full, sjeng, stockfish_full, stockfish, rating, legit, speed, player, game_id) =
    member this.Id = _id
    member this.Max = max
    member this.SjengFull = sjeng_full
    member this.Sjeng = sjeng
    member this.StockfishFull = stockfish_full
    member this.Stockfish = stockfish
    member this.Rating = rating
    member this.Legit = legit
    member this.Speed = speed
    member this.Player = player
    member this.GameId = game_id