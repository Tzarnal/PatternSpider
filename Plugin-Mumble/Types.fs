module Types

type ServerInfo = {
    IrcServer: string;
    IrcChannel: string;
    MumbleCVP: string;
}

type User = { Name: string; Deaf: bool }

type ChannelInfo = {
    Id: int64;
    Channels: ChannelInfo array;
    Name: string;
    Users: User array
    x_connecturl: string;
}

type RootElement = {
    Channels: ChannelInfo array
}

type ServerState = {
    Id: int64;
    Root: RootElement
    Name: string;
    x_connecturl: string;
}

    