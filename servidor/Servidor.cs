// Program.cs (Servidor)
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.Clear();

const int port = 5000;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"[SERVIDOR] Escuchando en {port}...");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(async () =>
    {
        var ep = client.Client.RemoteEndPoint;
        Console.WriteLine($"[SERVIDOR] Conexión: {ep}");
        using var stream = client.GetStream();
        var buffer = new byte[4096];
        try
        {
            int read;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, read).TrimEnd('\r', '\n');
                Console.WriteLine($"[SERVIDOR] {ep}: {msg}");
                if (msg.Equals("/salir", StringComparison.OrdinalIgnoreCase)) break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVIDOR] Error con {ep}: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine($"[SERVIDOR] Desconexión: {ep}");
        }
    });
}
