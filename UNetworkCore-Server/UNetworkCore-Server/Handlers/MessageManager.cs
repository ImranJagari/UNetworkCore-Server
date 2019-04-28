using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UNetworkCore_Core.Reflection;
using UNetworkCore_Protocol.Enums;
using UNetworkCore_Protocol.Messages;
using UNetworkCore_Server.Network;

namespace UNetworkCore_Server.Handlers
{
    public class MessageManager
    {
        public static bool isInitialized = false;
        public static Dictionary<MessageIdEnum, Action<Client, NetworkMessage>> Handlers = new Dictionary<MessageIdEnum, Action<Client, NetworkMessage>>();
        public static Dictionary<MessageIdEnum, MessageIdAttribute> Requirements = new Dictionary<MessageIdEnum, MessageIdAttribute>();

        public static void Initialize(Assembly asm)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                var methods = asm.GetTypes()
                          .SelectMany(t => t.GetMethods())
                          .Where(m => m.HasAttribute(typeof(MessageIdAttribute)));

                foreach (var method in methods)
                {
                    Handlers.Add(method.GetCustomAttribute<MessageIdAttribute>().MessageId, method.CreateDelegate<Action<Client, NetworkMessage>>());
                    Requirements.Add(method.GetCustomAttribute<MessageIdAttribute>().MessageId, method.GetCustomAttribute<MessageIdAttribute>());
                }
            }
        }
        public static void ParseHandler(Client client, NetworkMessage message)
        {
            try
            {
                if (message != null)
                {
                    if (Handlers.TryGetValue(message.MessageId, out var handler))
                    {
                        if (CheckRequirements(client, message))
                            handler(client, message);
                        else
                            Console.WriteLine(string.Format("Received packet without requirements validated : id = {0} -> {1}", message.MessageId, message));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Receive unknown Packet : id = {0} -> {1}", message.MessageId, message));
                    }
                }
                else
                {
                    Console.WriteLine("Receive empty packet !");
                    client.Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static bool CheckRequirements(Client client, NetworkMessage message)
        {
            return true;
        }
    }
}
