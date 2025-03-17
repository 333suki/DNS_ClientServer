﻿using System.Collections.Immutable;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using LibData;

// SendTo();
class Program {
    static void Main(string[] args) {
        ClientUDP.start();
    }
}

public class Setting {
    public int ServerPortNumber { get; set; }
    public string? ServerIPAddress { get; set; }
    public int ClientPortNumber { get; set; }
    public string? ClientIPAddress { get; set; }
}

static class ClientUDP {

    //TODO: [Deserialize Setting.json]
    static string configFile = @"Setting.json";
    static string configContent = File.ReadAllText(configFile);
    static Setting? setting = JsonSerializer.Deserialize<Setting>(configContent);
    private static Socket clientSocket;

    public static void start() {
        //TODO: [Create endpoints and socket]
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(setting.ClientIPAddress), setting.ClientPortNumber);
        clientSocket.Bind(clientEndPoint);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(setting.ServerIPAddress), setting.ServerPortNumber);
        

        //TODO: [Create and send HELLO]
        SendMessageToServer(new Message { MsgId = 1, MsgType = MessageType.Hello, Content = "Hello from client!!!" }, serverEndPoint);

        //TODO: [Receive and print Welcome from server]
        Message welcomeMessage = ReceiveMessageFromServer(serverEndPoint);
        Console.WriteLine($"Received message: {welcomeMessage.MsgId}, {welcomeMessage.MsgType}, {welcomeMessage.Content}");

        // TODO: [Create and send DNSLookup Message]
        SendMessageToServer(new Message { MsgId = 33, MsgType = MessageType.DNSLookup, Content = "www.outlook.com" }, serverEndPoint);

        //TODO: [Receive and print DNSLookupReply from server]


        //TODO: [Send Acknowledgment to Server]

        // TODO: [Send next DNSLookup to server]
        // repeat the process until all DNSLoopkups (correct and incorrect onces) are sent to server and the replies with DNSLookupReply

        //TODO: [Receive and print End from server]
    }



    private static void SendMessageToServer(Message message, EndPoint serverEndPoint)
    {
        try
        {
            // Convert message object to JSON string
            string jsonMessage = JsonSerializer.Serialize(message);
            // Convert JSON string to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            // Send the bytes to the server
            clientSocket.SendTo(messageBytes, serverEndPoint);
            Console.WriteLine($"Sent message: {message.MsgId}, {message.MsgType}, {message.Content}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    private static Message ReceiveMessageFromServer(EndPoint serverEndPoint)
    {
        byte[] buffer = new byte[1024];
        // Put received bytes in the buffer
        int bytesReceived = clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
        // Convert buffer to JSON string
        string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
        // Convert JSON string to Message object
        Message message = JsonSerializer.Deserialize<Message>(jsonMessage);
        return message;
    }
}
