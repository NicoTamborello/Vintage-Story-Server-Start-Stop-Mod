using System.Net;
using System.Text;
using Vintagestory.API.Server;
using Vintagestory.API.Common;

public class ShutdownNotifier : ModSystem
{
    private ICoreServerAPI sapi;
    private HttpListener listener;

    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;

        listener = new HttpListener();
        listener.Prefixes.Add("http://127.0.0.1:8888/shutdown-warning/");
        listener.Start();

        listener.BeginGetContext(OnRequestReceived, null);
    }

    private void OnRequestReceived(IAsyncResult ar)
    {
        var context = listener.EndGetContext(ar);
        listener.BeginGetContext(OnRequestReceived, null); // keep listening

        sapi.SendMessageToAllPlayers(EnumChatType.Notification, "[Server]: Server will shut down in 30 seconds. Please log out safely.");

        string responseString = "Shutdown warning sent.";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    public override void Dispose()
    {
        listener?.Stop();
        listener?.Close();
    }
}