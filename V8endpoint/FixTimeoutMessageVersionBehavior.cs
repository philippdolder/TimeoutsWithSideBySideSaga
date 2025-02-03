using NServiceBus.Pipeline;

namespace TheEndpoint;

public class FixTimeoutMessageVersionBehavior : Behavior<IIncomingPhysicalMessageContext>
{
    public override Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        if (context.Message.Headers.TryGetValue(Headers.IsSagaTimeoutMessage, out var isSagaTimeout))
        {
            var enclosedMessageTypes = context.Message.Headers[NServiceBus.Headers.EnclosedMessageTypes];
            if (enclosedMessageTypes == "TheEndpoint.TheTimeout, TheEndpoint, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                enclosedMessageTypes = string.Join(",", enclosedMessageTypes.Split(',').Take(2));
                context.Message.Headers[NServiceBus.Headers.EnclosedMessageTypes] = enclosedMessageTypes;
            }
            
            var sagaType = context.Message.Headers[NServiceBus.Headers.SagaType];
            if (sagaType == "TheEndpoint.TheSaga, TheEndpoint, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                // This is not really needed, it avoids a warning from the persister that a specific saga type cannot be found and it'll fall back to saga id 
                sagaType = string.Join(",", sagaType.Split(',').Take(2));
                context.Message.Headers[NServiceBus.Headers.SagaType] = sagaType;
            }
        }
        
        return next();
    }
}