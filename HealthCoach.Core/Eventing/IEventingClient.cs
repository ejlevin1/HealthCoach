using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCoach.Core.Eventing
{
    public interface IEventingClient
    {
        void Publish(dynamic evt);
    }
}
