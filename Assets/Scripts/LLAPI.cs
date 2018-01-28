using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ReceievedConnection{

	public int recHostId;
	public int recConnectionid;
	public int recChannelId;
	public byte[] recbuffer;
	public int bufferSize;
	public int dataSize;
	public byte error;
}
public enum HEADER_TYPES{
  
    QUATERNION,
    VECTOR3,
	TRANSFORM,
    INT,
    FLOAT,
    BYTE,
}

public class LLAPI : NetworkBehaviour {

    public Text recText;
	public String ipAddress;

    int myUnreliableChannelId;
	int myStateUpdateChannelId;
    int maxConnections = 10;
    int ServerSocketId = -1;
    int ClientSocketId = -1;
    int socketPort = 7777;

    int connectionId;
    public bool isConnectionServer;
	public bool isStart;

    public Rotate rCube;

    Quaternion HostCubeQuat;
    Quaternion RecCubeQuat;

    Vector3 HostCubePos;
    Vector3 RecCubePos;




    public Dictionary<int, int> ConnList;
    GlobalConfig gConfig;
	ReceievedConnection recConnection;
    NetworkEventType recNetworkEvent;

    void OnEnable () {
        rCube = GameObject.FindObjectOfType<Rotate>();
        NetworkTransport.Init();
        Init();
    }

    void Init()
    {
		recConnection.recbuffer = new byte[1024];
		recConnection.bufferSize = 1024;

        ConnList = new Dictionary<int, int>();

        ConnectionConfig config = new ConnectionConfig();
        config.MinUpdateTimeout = 1;

        gConfig = new GlobalConfig();
        gConfig.ThreadAwakeTimeout = 1;



        myUnreliableChannelId = config.AddChannel(QosType.Unreliable);
        myStateUpdateChannelId = config.AddChannel(QosType.StateUpdate);

        HostTopology topology = new HostTopology(config, maxConnections);

        ServerSocketId = NetworkTransport.AddHost(topology, socketPort);
        ClientSocketId = NetworkTransport.AddHost(topology);

        Debug.Log("Socket Open. ServerSocketId is: " + ServerSocketId);
        Debug.Log("Socket Open. ClientSocketId is: " + ClientSocketId);


    }

    void FixedUpdate () {
        do {
			recNetworkEvent = NetworkTransport.Receive(out recConnection.recHostId, out recConnection.recConnectionid, out recConnection.recChannelId, recConnection.recbuffer,
				recConnection.bufferSize, out recConnection.dataSize, out recConnection.error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    isStart = true;
				if (ServerSocketId == recConnection.recHostId)
                    {
                        isConnectionServer = true;

					if (!ConnList.ContainsKey(recConnection.recHostId) && !ConnList.ContainsValue(recConnection.recConnectionid))
                        {
						ConnList.Add(recConnection.recHostId, recConnection.recConnectionid);
                        Debug.Log(ConnList.Count);
                        }
                        rCube.ShowHostColour();

                        Debug.Log("This is Host");
                        Debug.Log("Server: Player " + connectionId.ToString() + " connected!");
                    }
				else if (ClientSocketId == recConnection.recHostId)
                    {
                        isConnectionServer = false;
                        Debug.Log("This is a Client!");
                        Debug.Log("Client: Client connected to: " + connectionId.ToString() + " connected!");
                    }
                    break;
                case NetworkEventType.DataEvent:


				if (ServerSocketId == recConnection.recHostId)
                    {
					CheckType(recConnection.recbuffer);

                        //	Debug.Log("incoming message event received: ");
                    }
				else if (ClientSocketId == recConnection.recHostId)
                    {
                        //	Debug.Log("incoming message event received");
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    isStart = false;
				if (ServerSocketId == recConnection.recHostId)
                    {

                        Debug.Log("Disconnecting Server: " + connectionId);
                    }
				else if (ClientSocketId == recConnection.recHostId)
                    {

                        Debug.Log("Disconnecting Client: " + connectionId);
                    }


				if (ConnList.ContainsKey(recConnection.recHostId) && ConnList.ContainsValue(recConnection.recConnectionid))
                    {
					ConnList.Remove(recConnection.recHostId);
                        Debug.Log(ConnList.Count);
                    }
                    break;
            }

         
        } while (recNetworkEvent != NetworkEventType.Nothing);

        if (!isConnectionServer && isStart)
        {
            {
                SerialiseTransform(rCube.transform);
            }
        }
    }

    public void Connect()
    {
        byte error;
        connectionId = NetworkTransport.Connect(ClientSocketId, ipAddress, socketPort, 0, out error);

        if (error != (byte)NetworkError.Ok)
        {
            NetworkError NetError = (NetworkError)error;
            Debug.Log("Error: " + NetError);
        }
        Debug.Log("Connected to server. ConnectionId: " + connectionId);

    }
    public void Disconnect()
    {
        byte error;

        NetworkTransport.Disconnect(ServerSocketId, connectionId, out error);

        if (error != (byte)NetworkError.Ok)
        {
            NetworkError NetError = (NetworkError)error;
            Debug.Log("Error: " + NetError);
        }
        Debug.Log("Disconnected from server. ConnectionId: " + connectionId);

    }

    public void SendSocketMessage(byte[] buffer)
    {
        byte error;

		NetworkTransport.Send(ClientSocketId, connectionId, myStateUpdateChannelId, buffer, buffer.Length, out error);

        if (error != (byte)NetworkError.Ok)
        {
            NetworkError NetError = (NetworkError)error;
            Debug.Log("Error: " + NetError);
        }
    }

    public void SendClientMessage(int clientIndex, byte[] buffer)
    {

		byte[] buffer1 = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, "HelloServer");

		byte error;
        NetworkTransport.Send(ClientSocketId, ConnList[clientIndex], myStateUpdateChannelId, buffer1, buffer.Length, out error);


        if (error != (byte)NetworkError.Ok)
        {
            NetworkError NetError = (NetworkError)error;
            Debug.Log("Error: " + NetError);
        }
    }
	public void SendClientsMessage(byte[] buffer)
	{

		byte error;
		for (int i = 0; i < ConnList.Count; i++) {
			NetworkTransport.Send (ClientSocketId, ConnList [i], myStateUpdateChannelId, buffer, buffer.Length, out error);

			if (error != (byte)NetworkError.Ok) {
				NetworkError NetError = (NetworkError)error;
				Debug.Log ("Error: " + NetError);
			}
		}
	}
    public void SerialiseTransform(Transform targetTransform)
    {
		SerializableTypes.TransformStruct sTransform = new SerializableTypes.TransformStruct (targetTransform);
		//Debug.Log (sTransform.ToArray ().Length);
		SendSocketMessage (sTransform.ToArray ());
	}

	public Vector3 DeSerializeTransformPosition(SerializableTypes.TransformStruct tStruct)
	{
		return tStruct.ToPosition();
	}
	public Quaternion DeSerializeTransformRotation(SerializableTypes.TransformStruct tStruct)
	{
		return tStruct.ToRotation();
	}

    public void CheckType(byte[] buffer )
    {
        int header = buffer[0];
        switch (header) {
            case (int)HEADER_TYPES.QUATERNION:       
		//	rCube.transform.rotation = DeSerializeRotation(buffer);
                break;
		case (int)HEADER_TYPES.VECTOR3:
			//rCube.transform.position = DeSerializePosition (buffer);
                break;
		case (int)HEADER_TYPES.TRANSFORM:
			SerializableTypes.TransformStruct tStructRec;
			tStructRec = SerializableTypes.TransformStruct.FromArray (buffer);

			rCube.recUpdatePos = DeSerializeTransformPosition (tStructRec);
			rCube.recUpdateQuat = DeSerializeTransformRotation (tStructRec);
			break;
        }
        
    }
   
    public void OnClickConnect()
    {
        Connect();
    }
    public void OnClickSend()
    {
		

    }
    public void OnClickDisconnect()
    {
        Disconnect();
    }

}
