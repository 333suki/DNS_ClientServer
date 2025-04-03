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
    // TODO: [Read the JSON file and return the list of DNSRecords]
    static Setting? setting = JsonSerializer.Deserialize<Setting>(File.ReadAllText(Path.Combine("Setting.json")));
    static List<DNSRecord>? savedRecords = JsonSerializer.Deserialize<List<DNSRecord>>(File.ReadAllText(Path.Combine("DNSrecords.json")));
    private static Socket serverSocket;
    
    public static void start() {
        // TODO: [Create a socket and endpoints and bind it to the server IP address and port number]
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(setting.ServerIPAddress), setting.ServerPortNumber);
        serverSocket.Bind(serverEndPoint);
        Console.WriteLine($"Server is listening on {setting.ServerIPAddress}:{setting.ServerPortNumber}");
        Console.WriteLine();

        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(setting.ClientIPAddress), setting.ClientPortNumber);

        while (true) {
            // TODO:[Receive and print a received Message from the client]
            Message message = ReceiveMessageFromClient(clientEndPoint);

            switch (message.MsgType) {
                case MessageType.Hello:
                    SendMessageToClient(new Message { MsgId = 4, MsgType = MessageType.Welcome, Content = "Welcome from server!!!" }, clientEndPoint);
                    break;
                // TODO:[Receive and print DNSLookup]
                case MessageType.DNSLookup:
                    // TODO:[Query the DNSRecord in Json file]
                    bool found = false;
                    foreach (DNSRecord record in savedRecords) {
                        if (message.Content is DNSRecord searchedRecord) {
                            if (String.Compare(record.Type.Trim(), searchedRecord.Type.Trim(), StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(record.Name.Trim(), searchedRecord.Name.Trim(), StringComparison.OrdinalIgnoreCase) == 0) {
                                // TODO:[If found Send DNSLookupReply containing the DNSRecord]
                                found = true;
                                SendMessageToClient(new Message { MsgId = message.MsgId, MsgType = MessageType.DNSLookupReply, Content = new DNSRecord { Type = record.Type, Name = record.Name, Value = record.Value, TTL = record.TTL, Priority = record.Priority } }, clientEndPoint);
                                break;
                            }
                        }
                    }
                    // TODO:[If not found Send Error]
                    if (!found) {
                        SendMessageToClient(new Message { MsgId = 753444, MsgType = MessageType.Error, Content = "Domain not found" }, clientEndPoint);
                    }
                    
                    // TODO:[Receive Ack about correct DNSLookupReply from the client]
                    Message ackMessage = ReceiveMessageFromClient(clientEndPoint);
                    break;
                case MessageType.Ack:
                    break;
                case MessageType.End:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Client disconnected ({clientEndPoint})");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
        }
    }

    private static void SendMessageToClient(Message message, EndPoint clientEndPoint) {
        try {
            // Convert message object to JSON string
            string jsonMessage = JsonSerializer.Serialize(message);
            // Convert JSON string to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            // Send the bytes to the client
            serverSocket.SendTo(messageBytes, clientEndPoint);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Sent message: ");   
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.WriteLine();
        } catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Error sending message: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ex.Message);
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

        using (JsonDocument doc = JsonDocument.Parse(jsonMessage)) {
            if (doc.RootElement.TryGetProperty("Content", out JsonElement contentElement)) {
                try {
                    string contentJson = contentElement.GetRawText();
                    message.Content = JsonSerializer.Deserialize<DNSRecord>(contentJson);
                }
                catch (Exception e) { }
            }
        }

        if (message.MsgType == MessageType.Hello) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Client connected ({clientEndPoint})");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Received message: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.WriteLine();
        return message;
    }
}
