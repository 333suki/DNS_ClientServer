using System;
using System.Data;
using System.Data.SqlTypes;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using LibData;

// ReceiveFrom();
class Program {
    static void Main(string[] args) {
        ServerUDP.start();
    }
}

public class Setting {
    public int ServerPortNumber { get; set; }
    public string? ServerIPAddress { get; set; }
    public int ClientPortNumber { get; set; }
    public string? ClientIPAddress { get; set; }
}

static class ServerUDP {
    static string configFile = @"Setting.json";
    static string configContent = File.ReadAllText(configFile);
    static Setting? setting = JsonSerializer.Deserialize<Setting>(configContent);
    private static Socket serverSocket;

    // TODO: [Read the JSON file and return the list of DNSRecords]


    public static void start() {
        // TODO: [Create a socket and endpoints and bind it to the server IP address and port number]
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(setting.ServerIPAddress), setting.ServerPortNumber);
        serverSocket.Bind(serverEndPoint);
        Console.WriteLine($"Server is listening on {setting.ServerIPAddress}:{setting.ServerPortNumber}");

        // TODO:[Receive and print a received Message from the client]
        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(setting.ClientIPAddress), setting.ClientPortNumber);
        
        // TODO:[Receive and print Hello]
        Message helloMessage = ReceiveMessageFromClient(clientEndPoint);
        Console.WriteLine(helloMessage.Content);

        // TODO:[Send Welcome to the client]

        // TODO:[Receive and print DNSLookup]

        // TODO:[Query the DNSRecord in Json file]

        // TODO:[If found Send DNSLookupReply containing the DNSRecord]

        // TODO:[If not found Send Error]

        // TODO:[Receive Ack about correct DNSLookupReply from the client]

        // TODO:[If no further requests receieved send End to the client]
    }

    private static void SendMessageToClient(Message message, EndPoint clientEndPoint) {
        try {
            // Convert message object to JSON string
            string jsonMessage = JsonSerializer.Serialize(message);
            // Convert JSON string to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            // Send the bytes to the client
            serverSocket.SendTo(messageBytes, clientEndPoint);
        } catch (Exception ex) {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    private static Message ReceiveMessageFromClient(EndPoint clientEndPoint) {
        byte[] buffer = new byte[1024];
        // Put received bytes in the buffer
        int bytesReceived = serverSocket.ReceiveFrom(buffer, ref clientEndPoint);
        // Convert buffer to JSON string
        string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
        // Convert JSON string to Message object
        Message message = JsonSerializer.Deserialize<Message>(jsonMessage);
        return message;
    }
}
