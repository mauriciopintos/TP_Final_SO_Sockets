# TP Final de Sistemas Operativos - UNaHur 2025C2
## Proyecto de Sockets 
---    

**Alumno:**     PINTOS, Mauricio    

**Docente:**    Ing. Gabriel Esquivel    
**Materia:**    Sistemas Operativos    
**Año:**        2025    
**Revisión:**   1.0    

---

## Explicación Completa del Cliente y Servidor TCP en C#

Este documento explica detalladamente cómo funciona este ejemplo de un **Cliente** y un **Servidor TCP** implementados en ***C#***, para el TP Final de la materia **Sistemas Operativos** de la **Universidad Nacional de Hurlingham** (UNaHur) 2025C2.

---

# 📌 CÓDIGO DEL CLIENTE

**Ver en:** `./cliente/Cliente.cs`

---

## 🧠 Explicación paso a paso del Cliente

### 1) `using` de namespaces

- `System.Net.Sockets`: provee las clases para conectar por TCP.
- `System.Text`: permite convertir texto ↔ bytes con UTF-8.

---

### 2) Selección de host y puerto

```csharp
string host = args.Length > 0 ? args[0] : "127.0.0.1";
const int port = 5000;
```

- Si el usuario pasa una IP como argumento → se usa esa IP.
- Si no pasa nada → se usa `"127.0.0.1"` (localhost).
- Puerto fijo `5000` → debe coincidir con el servidor.

---

### 3) Crear el cliente y conectarse

```csharp
using var client = new TcpClient();
await client.ConnectAsync(host, port);
```

- `TcpClient` representa la conexión TCP.
- `ConnectAsync` **establece una conexión TCP** (*handshake TCP*).
- `await` evita bloquear el hilo principal.

---

### 4) Obtener el stream de comunicación

```csharp
using var stream = client.GetStream();
```

- Devuelve un `NetworkStream`.
- Ese stream es el canal de lectura/escritura hacia el servidor.

---

### 5) Enviar mensajes al servidor

```csharp
while (true)
{
    var linea = Console.ReadLine() ?? "";
    var data = Encoding.UTF8.GetBytes(linea + "\n");
    await stream.WriteAsync(data, 0, data.Length);
    if (linea.Equals("/salir", StringComparison.OrdinalIgnoreCase)) break;
}
```

- Lee lo que el usuario escribe.
- Lo convierte en bytes.
- Lo envía por el stream.
- Si escribe `/salir`, sale del loop y finaliza.

---

# 🖥️ CÓDIGO DEL SERVIDOR

**Ver en:** `./servidor/Servidor.cs`

---

## 🧠 Explicación paso a paso del Servidor

### 1) Preparar el servidor

```csharp
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
```

- `TcpListener` abre el puerto `5000`.
- `Any` = escucha en todas las interfaces (localhost + red local).
- `Start()` inicia la escucha.

---

### 2) Loop principal aceptando clientes

```csharp
var client = await listener.AcceptTcpClientAsync();
```

- Espera (sin bloquear el hilo) a que un cliente se conecte.
- Cuando llega uno, devuelve un nuevo `TcpClient`.

Luego se usa:

```csharp
_ = Task.Run(async () => { ... });
```

- Cada cliente se atiende en una **tarea paralela**.
- Permite atender **múltiples clientes simultáneos**.

---

### 3) Manejo del cliente

```csharp
var ep = client.Client.RemoteEndPoint;
using var stream = client.GetStream();
```

- `RemoteEndPoint`: IP y puerto del cliente.
- `GetStream()`: igual que en el cliente, el canal de comunicación.

---

### 4) Recibir mensajes

```csharp
while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
    var msg = Encoding.UTF8.GetString(buffer, 0, read).TrimEnd('\r', '\n');
    Console.WriteLine($"[SERVIDOR] {ep}: {msg}");
    if (msg.Equals("/salir", StringComparison.OrdinalIgnoreCase)) break;
}
```

- Lee bytes enviados por el cliente.
- Los decodifica a texto con UTF-8.
- Muestra lo recibido.
- Si recibe `/salir`, corta la conexión.
- Con `StringComparison.OrdinalIgnoreCase` compara caracteres ignorando mayusculas y minusculas.

---

### 5) Cierre limpio

```csharp
finally
{
    client.Close();
    Console.WriteLine($"[SERVIDOR] Desconexión: {ep}");
}
```

- Libera el socket.
- Loguea la desconexión.

---

# 🧩 Qué conceptos se aplicaron en el desarrollo

- Uso de **socket TCP**
- Par **cliente** / **servidor**
- `TcpClient` vs `TcpListener`
- Uso de llamada **no bloqueante**
- Manejode **stream** (flujo de bytes)
- Convertir **texto ↔ bytes**
- Concurrencia basica mediante el uso de **Task.Run**
- Comunicación entre máquinas en una **LAN**
- Uso de comandos (como `/salir`)

---

# ✔️ Cómo probarlo

### 1) Probar en la misma máquina
- Abrir **dos terminales** en el path donde estan las carpetas **Cliente y Servidor**.
- Terminal 1:
  ```
  dotnet run --project Servidor
  ```
- Terminal 2:
  ```
  dotnet run --project Cliente -- 127.0.0.1
  ```

### 2) Probar entre dos máquinas en LAN
- En la máquina del servidor:
- Abrir la **terminal** en el path donde esta la carpeta **Servidor**.
  ```
  ipconfig
  ```
- Buscar la IP: por ejemplo `192.168.1.50`.
- Iniciar el Servidor:
  ```
  dotnet run --project Servidor
  ```
    
- En el cliente:
- Abrir la **terminal** en el path donde esta la carpeta **Cliente**.
- Iniciar el Cliente:
  ```
  dotnet run --project Cliente -- 192.168.1.50
  ```

---
