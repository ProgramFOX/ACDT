namespace AntichessCheatDetection.Modules.Investigate

open Newtonsoft.Json

open System

type UnixDateTimeConverter() =
    inherit JsonConverter()

    override this.CanConvert t =
        t = typeof<DateTime>
    
    override this.WriteJson(writer, value, serializer) =
        raise(NotImplementedException())
    
    override this.ReadJson(reader, objectType, existingValue, serializer) =
        let unixMs = Int64.Parse(string reader.Value)
        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds(unixMs).DateTime, TimeZoneInfo.Utc) :> obj