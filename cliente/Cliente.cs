using System.Net.Sockets;
using System.Text;

Console.Clear();

string host = args.Length > 0 ? args[0] : "127.0.0.1"; // LocalHost IP por default
const int port = 5000;

using var client = new TcpClient();

Console.WriteLine($"[CLIENTE] Conectando a {host}:{port} ...");
await client.ConnectAsync(host, port);
Console.WriteLine("""
===============================================
* AYUDA:                                      *
*---------------------------------------------*
* ENTER para enviar mensaje.                  *
* '/salir' y ENTER, para finalizar.           *
===============================================

[CLIENTE] Conectado.
""");

using var stream = client.GetStream();

while (true)
{
    Console.Write("<CLIENTE> ");
    var linea = Console.ReadLine() ?? "";
    var data = Encoding.UTF8.GetBytes(linea + "\n");
    await stream.WriteAsync(data, 0, data.Length);
    if (linea.Equals("/salir", StringComparison.OrdinalIgnoreCase)) break;
}
