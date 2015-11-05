using System.ServiceModel;
using RedBubbleObjects;

namespace stylus_create
{
    [ServiceContract]
    public interface IStylus_Create
    {
        [OperationContract]
        oCreateOrderResponse PostOrder(oCreateOrderRequest order);
    }
}
