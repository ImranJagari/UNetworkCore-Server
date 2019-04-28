using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UNetworkCore_Core.IO;
using UNetworkCore_Core.Reflection;
using UNetworkCore_Protocol.Enums;
using UNetworkCore_Protocol.Messages;

namespace UNetworkCore_Protocol
{
    public static class MessageReceiver
    {
        private static readonly Dictionary<MessageIdEnum, Type> Messages = new Dictionary<MessageIdEnum, Type>();
        private static readonly Dictionary<MessageIdEnum, Func<NetworkMessage>> Constructors = new Dictionary<MessageIdEnum, Func<NetworkMessage>>();

        public static void Initialize()
        {
            var assembly = Assembly.GetAssembly(typeof(MessageReceiver));
            foreach (var type in assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(NetworkMessage))))
            {
                var field = type.GetField("Id");
                if (field != null)
                {
                    var num = (MessageIdEnum)field.GetValue(type);
                    if (Messages.ContainsKey(num))
                    {
                        throw new AmbiguousMatchException(
                            $"MessageReceiver() => {num} item is already in the dictionary, old type is : {Messages[num]}, new type is  {type}");
                    }
                    Messages.Add(num, type);
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception($"'{type}' doesn't implemented a parameterless constructor");
                    }
                    Constructors.Add(num, constructor.CreateDelegate<Func<NetworkMessage>>());
                }
            }
        }

        public static NetworkMessage BuildMessage(MessageIdEnum id, IDataReader reader)
        {
            if (!Messages.ContainsKey(id))
            {
                return null;
            }
            var message = Constructors[id]();
            if (message == null)
            {
                return null;
            }
            message.Unpack(reader);
            return message;
        }

        public static Type GetMessageType(MessageIdEnum id)
        {
            if (!Messages.ContainsKey(id))
            {
                return null;
            }
            return Messages[id];
        }

        [Serializable]
        public class MessageNotFoundException : Exception
        {
            public MessageNotFoundException()
            {
            }

            public MessageNotFoundException(string message)
                : base(message)
            {
            }

            public MessageNotFoundException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected MessageNotFoundException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
