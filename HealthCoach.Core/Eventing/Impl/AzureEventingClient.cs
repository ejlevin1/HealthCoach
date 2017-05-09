//using System;
//using System.Collections.Generic;
//using System.Text;
//using Amqp;
//using Newtonsoft.Json;

//namespace HealthCoach.Core.Eventing.Impl
//{
//    public class AzureEventingClient : IEventingClient
//    {
//        private Session _session = null;

//        public AzureEventingClient(string connectionString)
//        {
//            Connection connection = Connection.Factory.CreateAsync(new Address(connectionString)).Result;
//            _session = new Session(connection);
//        }

//        public void Publish(dynamic evt)
//        {
//            TODO Check if the connection is closed.

//            SenderLink amqpSender = new SenderLink(_session, "topic-sender-link", "entity-01");

//            var message = new Message(JsonConvert.SerializeObject(evt));

//            amqpSender.Send(message, (msg, outcome, state) =>
//            {
//                Console.WriteLine("Test");
//            }, null);
//        }
//    }
//}
