using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain;

using MassTransit;

namespace Infrastructure.Contracts_Implemintaion;
public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
        Console.WriteLine($"[SUCCESS] Received Event: {message.Email}");
    }
}
