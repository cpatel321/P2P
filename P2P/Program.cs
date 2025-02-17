using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class P2PFileTransfer
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: \nSender: P2PFileTransfer sender <receiver_ip> <file_path>\nReceiver: P2PFileTransfer receiver");
            return;
        }

        string mode = args[0].ToLower();

        if (mode == "sender" && args.Length == 3)
        {
            string ip = args[1];
            string filePath = args[2];
            await SendFile(ip, filePath);
        }
        else if (mode == "receiver")
        {
            await StartReceiver();
        }
        else
        {
            Console.WriteLine("Invalid arguments.");
        }
    }

    static async Task SendFile(string ip, string filePath)
    {
        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(ip, 5000);
            NetworkStream stream = client.GetStream();
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
            Console.WriteLine("File sent successfully.");
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task StartReceiver()
    {
        try
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Waiting for connection...");
            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream stream = client.GetStream();
            using FileStream fileStream = new FileStream("received.txt", FileMode.Create);
            await stream.CopyToAsync(fileStream);
            Console.WriteLine("File received successfully.");
            client.Close();
            listener.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
