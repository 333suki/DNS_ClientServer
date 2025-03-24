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
    static Setting? setting = JsonSerializer.Deserialize<Setting>(File.ReadAllText("Setting.json"));
    static List<DNSRecord>? records = JsonSerializer.Deserialize<List<DNSRecord>>(File.ReadAllText("DNSrecords.json"));
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
        Console.WriteLine($"Received message: {helloMessage}");

        // TODO:[Send Welcome to the client]
        SendMessageToClient(new Message{MsgId = 4, MsgType = MessageType.Welcome, Content = "Welcome from server!!!"}, clientEndPoint);
        
        while (true) {
            // TODO:[Receive and print DNSLookup]
            Message dnsLookUpMessage = ReceiveMessageFromClient(clientEndPoint);
            Console.WriteLine($"Received lookup message: {dnsLookUpMessage}");
            
            if (dnsLookUpMessage.MsgType == MessageType.DNSLookup) {
                // TODO:[Query the DNSRecord in Json file]
                bool found = false;
                foreach (DNSRecord record in records) {
                    if (String.Compare(record.Name.Trim(), dnsLookUpMessage.Content.ToString().Trim(), StringComparison.OrdinalIgnoreCase) == 0) {
                        // TODO:[If found Send DNSLookupReply containing the DNSRecord]
                        found = true;
                        SendMessageToClient(new Message { MsgId = dnsLookUpMessage.MsgId, MsgType = MessageType.DNSLookupReply, Content = new DNSRecord { Type = record.Type, Name = record.Name, Value = record.Value, TTL = record.TTL, Priority = record.Priority } }, clientEndPoint);
                        break;
                    }
                }

                // TODO:[If not found Send Error]
                if (!found) {
                    SendMessageToClient(new Message { MsgId = 753444, MsgType = MessageType.Error, Content = "Domain not found" }, clientEndPoint);
                }

                // TODO:[Receive Ack about correct DNSLookupReply from the client]
                Message ackMessage = ReceiveMessageFromClient(clientEndPoint);
                Console.WriteLine($"Received ack message: {ackMessage}");
            }
        }
        // TODO:[If end is received from client, stop connection]
    }

    private static void SendMessageToClient(Message message, EndPoint clientEndPoint) {
        try {
            // Convert message object to JSON string
            string jsonMessage = JsonSerializer.Serialize(message);
            // Convert JSON string to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            // Send the bytes to the client
            serverSocket.SendTo(messageBytes, clientEndPoint);
            Console.WriteLine($"Sent message: {message}");
            Console.WriteLine();
        } catch (Exception ex) {
            Console.WriteLine($"Error sending message: {ex.Message}");
            Console.WriteLine();
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
