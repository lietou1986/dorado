using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Core.Contracts
{
    [ServiceContract(Namespace = "urn:Dorado.platformservices")]
    public interface IBootstrap
    {
        [OperationContract]
        HostMetadata GetMetadata(string hostName, string applicationName, string machineName, string optionServiceName);
    }

    [ServiceContract(Namespace = "urn:Dorado.platformservices")]
    public interface IDomainLoader
    {
        [OperationContract]
        void UnloadDomain(string hostName, string appDomainHostName);

        [OperationContract]
        void ReloadDomain(string hostName, string appDomainHostName, HostMetadata metadata);

        [OperationContract]
        void LoadDomain(string hostName, string appDomainHostName, HostMetadata metadata);

        [OperationContract]
        List<ServiceMetadataBase> GetHostedServices(string hostName);
    }

    [ServiceContract(Namespace = "urn:Dorado.platformservices")]
    public interface IGenericContract
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Message ProcessMessage(Message message);
    }

    [ServiceContract(Namespace = "urn:Dorado.platformservices", SessionMode = SessionMode.NotAllowed)]
    public interface IBroadcast
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void ProcessMessage(Message message);
    }
}