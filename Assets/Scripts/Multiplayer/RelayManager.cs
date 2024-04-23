using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DilmerGames.Core.Singletons;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;

public class RelayManager : Singleton<RelayManager>
{
    [HideInInspector]
    public string joinCode;

    UnityTransport Transport => GetComponent<UnityTransport>();


    public async Task<bool> SetupRelay()
    {
        Allocation allocation;
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(2);
        }
        catch(RelayServiceException e)
        {
            Debug.Log("RelayManager.SetupRelay.CreateAllocationAsync: " + e.Message);
            return false;
        }
       

        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        try
        {
            relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);
            joinCode = relayHostData.JoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log("RelayManager.SetupRelay.GetJoinCodeAsync: " + e.Message);
            return false;
        }
       
        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes, relayHostData.Key, relayHostData.ConnectionData);

        return true;
    }

    public async Task<bool> JoinRelay(string joiningCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joiningCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log("RelayManager.JoinRelay.JoinAllocationAsync: " + e.Message);
            return false;
        }
        
        RelayJoinData relayJoinData = new RelayJoinData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            JoinCode = joiningCode
        };
        joinCode = joiningCode;
        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes, relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);
        
        return true;
    }

    public struct RelayJoinData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public System.Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public System.Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }
}