using System.Collections;
using System.Collections.Generic;

namespace Dorado.Platform.Events
{
    public interface IEventBus
    {
        IEnumerable Notify(string messageName, IDictionary<string, object> eventData);
    }
}