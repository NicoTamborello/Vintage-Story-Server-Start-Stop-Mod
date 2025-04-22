using System.Net;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace GracefulShutdown
{
    public class ModSystem_GracefulShutdown : ModSystem
    {
        private ICoreServerAPI sapi;
        private HttpListener listener;

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;

            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8888/shutdown/");
            listener.Start();
            listener.BeginGetContext(OnRequestReceived, null);

            api.Logger.Notification("GracefulShutdown mod is active and listening on /shutdown/");
        }

        private void OnRequestReceived(IAsyncResult ar)
        {
            HttpListenerContext context = listener.EndGetContext(ar);
            listener.BeginGetContext(OnRequestReceived, null); // Keep listening

            sapi.Event.EnqueueMainThreadTask(() =>
            {
                sapi.SendMessageToAllPlayers(EnumChatType.Notification, "[Server]: Saving world and shutting down. Please stand by...");
                sapi.WorldManager.SaveWorld(); // Save the world
                sapi.ShutdownServer();         // Gracefully shut down
            }, "GracefulShutdown");

            // Respond to curl
            string response = "Shutdown initiated.";
            byte[] buffer = Encoding.UTF8.GetBytes(response);
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
}
