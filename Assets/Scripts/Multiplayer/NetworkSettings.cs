using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSettings : Singleton<NetworkSettings>
{
    public enum SignInType { Anonymous, Oculus }

    public NetworkSettingsData data;

    public static ServerSettignsData serverData;//set on MultiplayManager

    public static readonly int maxHostConnections = 2;
     
    public static bool updateGame;

    public static ErrorType disconnectError;

    public enum ErrorType { HostFull, HostDisconnected, HostError, JoinError }

    private void Awake()
    {
        serverData = new ServerSettignsData();
    }


    public static ClientRpcParams GetCRP(List<ulong> clientIDs)
    {
        return new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = clientIDs } };
    }


    public void ResetData()
    {
        serverData = new ServerSettignsData();
    }

    public static string GetRandomServerName()
    {
        int randomName = Random.Range(0, serverNames.Length);
        return serverNames[randomName];
    }

    static string[] serverNames = new string[] {
        "Abominable",
        "Adventure",
        "Alaska",
        "Alpine",
        "Altitude",
        "Antarctic",
        "Arctic",
        "Ascent",
        "Aurora",
        "Avalanche",
        "Beanbag",
        "Beanie",
        "Belly Slide",
        "Below Zero",
        "Berg",
        "Big Foot",
        "Big Snow",
        "Big Surf",
        "Blizzard",
        "Bobsled",
        "Bonza",
        "Boots",
        "Breeze",
        "Brumby",
        "Bubblegum",
        "Bunny Hill",
        "Cabin",
        "Canoe",
        "Caribou",
        "Chinook",
        "Christmas",
        "Cloudy",
        "Cold Front",
        "Cold Snap",
        "Cozy",
        "Cream Soda",
        "Crunch",
        "Crystal",
        "Deep Freeze",
        "Deep Snow",
        "Down Under",
        "Downhill",
        "Dry Ice",
        "Elevation",
        "Fiesta",
        "Fjord",
        "Flippers",
        "Flurry",
        "Fog",
        "Freezer",
        "Frostbite",
        "Frosty",
        "Frozen",
        "Glacial",
        "Glacier",
        "Grasshopper",
        "Great White",
        "Grizzly",
        "Half Pipe",
        "Hibernate",
        "Hockey",
        "Hot Chocolate",
        "Husky",
        "Hypothermia",
        "Ice Age",
        "Ice Berg",
        "Ice Box",
        "Ice Breaker",
        "Ice Cave",
        "Ice Cold",
        "Ice Cream",
        "Ice Cube",
        "Ice Pack",
        "Ice Palace",
        "Ice Pond",
        "Ice Rink",
        "Ice Shelf",
        "Icebound",
        "Iceland",
        "Icicle",
        "Jack Frost",
        "Jackhammer",
        "Klondike",
        "Kosciuszko",
        "Mammoth",
        "Marshmallow",
        "Matterhorn",
        "Migrator",
        "Misty",
        "Mittens",
        "Mountain",
        "Mukluk",
        "Mullet",
        "North Pole",
        "Northern Lights",
        "Outback",
        "Oyster",
        "Parka",
        "Patagonia",
        "Permafrost",
        "Pine Needles",
        "Polar",
        "Polar Bear",
        "Powder Ball",
        "Puddle",
        "Rainbow",
        "Rocky Road",
        "Sabertooth",
        "Sardine",
        "Sasquatch",
        "Sherbet",
        "Shiver",
        "Skates",
        "Sled",
        "Sleet",
        "Slippers",
        "Slushy",
        "Snow Angel",
        "Snow Ball",
        "Snow Bank",
        "Snow Board",
        "Snow Cone",
        "Snow Covered",
        "Snow Day",
        "Snow Drift",
        "Snow Flake",
        "Snow Fort",
        "Snow Globe",
        "Snow Plow",
        "Snow Shoe",
        "Snowbound",
        "Snowcap",
        "Snowfall",
        "Snowmobile",
        "Snowy River",
        "South Pole",
        "Southern Lights",
        "Sparkle",
        "Sub Zero",
        "Summit",
        "Tea",
        "Thermal",
        "Toboggan",
        "Tundra",
        "Tuxedo",
        "Vanilla",
        "Walrus",
        "White House",
        "White Out",
        "Wind Chill",
        "Winter Land",
        "Wool Socks",
        "Yeti",
        "Yukon",
        "Zipline" };
}

[System.Serializable]
public class NetworkSettingsData
{
    public NetworkSettings.SignInType signInType = NetworkSettings.SignInType.Anonymous;

    public string environment = "development";

    public string versionNumber = "0.0.0";

    public string displayName;
}

[System.Serializable]
public class ServerSettignsData
{
    public bool isHost;
    public bool isPrivate;

    public string lobbyCode;
    public string joinCode;

    public string name;
}
