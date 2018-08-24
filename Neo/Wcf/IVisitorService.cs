using System;
using System.ServiceModel;

namespace NeoVisitor.Wcf
{
    [ServiceContract]
    public interface IVisitorService
    {
        [OperationContract]
        bool IsWellVisitor(Int64 visitorid, string visitorguid, string channelcode, out string msg);
    }
}