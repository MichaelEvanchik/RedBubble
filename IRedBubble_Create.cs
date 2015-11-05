using System.ServiceModel;
using RedBubbleObjects;

namespace redbubble_create
{
    [ServiceContract]
    public interface IRedBubble_Create
    {
        [OperationContract]
        oCreateOrderResponse PostOrder(oCreateOrderRequest order);
    }
}
