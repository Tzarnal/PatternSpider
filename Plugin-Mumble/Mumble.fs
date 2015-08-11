namespace Plugin_Mumble

open System.Collections.Generic;
open System.ComponentModel.Composition;
open PatternSpider.Irc;
open PatternSpider.Plugins;
open Newtonsoft.Json
open FSharp.Data
open System
open System.IO
open Types

[<Export(typeof<IPlugin>)>]
type Mumble() =
    let urls = Dictionary<string, string>()
    
    let setup = 
        let path = Path.Combine("Plugins", "Mumble", "Servers.json")
        let createKey (s: ServerInfo) =
            sprintf "%s::%s" (s.IrcServer.ToLowerInvariant()) (s.IrcChannel.ToLowerInvariant())
        let servers = JsonConvert.DeserializeObject<ServerInfo array>(File.ReadAllText(path))
        servers |> Seq.iter (fun s -> urls.Add(createKey s, s.MumbleCVP))
        ()

    do setup

    let getUrl (channel: string) =
        let c = channel.ToLowerInvariant()
        if (urls.ContainsKey(c)) then
            Some urls.[c]
        else
            None 

    let formatChannel (c: ChannelInfo) =
        let names = match c.Users.Length with
                    | 0 -> String.Empty
                    | 1 -> c.Users.[0].Name
                    | _ -> String.Join(", ", (c.Users |> Seq.map (fun x -> x.Name)))
        sprintf "%s - %s" c.Name names

    let formatOutput status =
        let root = status.Root
        let rec collectChannels (channels: ChannelInfo array) = 
            channels |> Seq.fold (fun x y -> x @ [y] @ (y.Channels |> collectChannels)) []

        let results = List<string>()
        results.Add(sprintf "%s - %s" status.Name status.x_connecturl)

        let channels = collectChannels root.Channels

        channels 
        |> List.filter (fun c -> c.Users.Length > 0)
        |> List.iter (fun c -> results.Add(formatChannel c))
        results
    
    let fetchMumbleStats (bot: IrcBot) (server: string) (message: IrcMessage) =
        let status = 
            try
                let response = Http.RequestString("https://www.mumbleboxes.com/servers/136/cvp.json")
                Some(JsonConvert.DeserializeObject<ServerState>(response))
            with
                | :? System.Exception as e -> printfn "%A" e; None
        
        match status with
        | Some root -> formatOutput root
        | None -> List<string>()

    interface IPlugin with
        member this.Name with get() = "Mumble"
        member this.Description with get() = "Shows information about the configured mumble server"
        member this.Commands with get() = List<string>([|"mumble"|])
        member this.OnChannelMessage(bot, server, channel, message) = List<string>()
        member this.OnUserMessage(bot, server, message) = List<string>()
        member this.IrcCommand (bot, server, message) = 
            let key = sprintf "%s::%s" (message.Server.ToLowerInvariant()) (message.Channel.ToLowerInvariant())
            match (getUrl key) with
            | Some c -> fetchMumbleStats bot server message
            | None -> List<string>()