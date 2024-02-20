using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// A wrapper around <see cref="ISession"/> to prevent accidental destruction of the Session.
/// </summary>
/// <param name="Session">The <see cref="ISession"/> we are wrapping.</param>
internal sealed record class WrappedSession(ISession Session) : ISession, IDisposable
{
    private bool disposed;

    /// <inheritdoc/>
    public event KeepAliveEventHandler KeepAlive
    {
        add => Session.KeepAlive += value;
        remove => Session.KeepAlive -= value;
    }

    /// <inheritdoc/>
    public event NotificationEventHandler Notification
    {
        add => Session.Notification += value;
        remove => Session.Notification -= value;
    }

    /// <inheritdoc/>
    public event PublishErrorEventHandler PublishError
    {
        add => Session.PublishError += value;
        remove => Session.PublishError -= value;
    }

    /// <inheritdoc/>
    public event PublishSequenceNumbersToAcknowledgeEventHandler PublishSequenceNumbersToAcknowledge
    {
        add => Session.PublishSequenceNumbersToAcknowledge += value;
        remove => Session.PublishSequenceNumbersToAcknowledge -= value;
    }

    /// <inheritdoc/>
    public event RenewUserIdentityEventHandler RenewUserIdentity
    {
        add => Session.RenewUserIdentity += value;
        remove => Session.RenewUserIdentity -= value;
    }

    /// <inheritdoc/>
    public event EventHandler SessionClosing
    {
        add => Session.SessionClosing += value;
        remove => Session.SessionClosing -= value;
    }

    /// <inheritdoc/>
    public event EventHandler SessionConfigurationChanged
    {
        add => Session.SessionConfigurationChanged += value;
        remove => Session.SessionConfigurationChanged -= value;
    }

    /// <inheritdoc/>
    public event EventHandler SubscriptionsChanged
    {
        add => Session.SubscriptionsChanged += value;
        remove => Session.SubscriptionsChanged -= value;
    }

    /// <inheritdoc/>
    public bool CheckDomain => Session.CheckDomain;

    /// <inheritdoc/>
    public ConfiguredEndpoint ConfiguredEndpoint => Session.ConfiguredEndpoint;

    /// <inheritdoc/>
    public bool Connected => Session.Connected;

    /// <inheritdoc/>
    public IReadOnlyDictionary<NodeId, DataDictionary> DataTypeSystem => Session.DataTypeSystem;

    /// <inheritdoc/>
    public Subscription DefaultSubscription
    {
        get => Session.DefaultSubscription;
        set => Session.DefaultSubscription = value;
    }

    /// <inheritdoc/>
    public int DefunctRequestCount => Session.DefunctRequestCount;

    /// <inheritdoc/>
    public bool DeleteSubscriptionsOnClose
    {
        get => Session.DeleteSubscriptionsOnClose;
        set => Session.DeleteSubscriptionsOnClose = value;
    }

    /// <inheritdoc/>
    public bool Disposed => Session.Disposed;

    /// <inheritdoc/>
    public EndpointDescription Endpoint => Session.Endpoint;

    /// <inheritdoc/>
    public EndpointConfiguration EndpointConfiguration => Session.EndpointConfiguration;

    /// <inheritdoc/>
    public IEncodeableFactory Factory => Session.Factory;

    /// <inheritdoc/>
    public FilterContext FilterContext => Session.FilterContext;

    /// <inheritdoc/>
    public int GoodPublishRequestCount => Session.GoodPublishRequestCount;

    /// <inheritdoc/>
    public object Handle => Session.Handle;

    /// <inheritdoc/>
    public IUserIdentity Identity => Session.Identity;

    /// <inheritdoc/>
    public IEnumerable<IUserIdentity> IdentityHistory => Session.IdentityHistory;

    /// <inheritdoc/>
    public int KeepAliveInterval
    {
        get => Session.KeepAliveInterval;
        set => Session.KeepAliveInterval = value;
    }

    /// <inheritdoc/>
    public bool KeepAliveStopped => Session.KeepAliveStopped;

    /// <inheritdoc/>
    public DateTime LastKeepAliveTime => Session.LastKeepAliveTime;

    /// <inheritdoc/>
    public IServiceMessageContext MessageContext => Session.MessageContext;

    /// <inheritdoc/>
    public int MinPublishRequestCount
    {
        get => Session.MinPublishRequestCount;
        set => Session.MinPublishRequestCount = value;
    }

    /// <inheritdoc/>
    public NamespaceTable NamespaceUris => Session.NamespaceUris;

    /// <inheritdoc/>
    public INodeCache NodeCache => Session.NodeCache;

    /// <inheritdoc/>
    public ITransportChannel NullableTransportChannel => Session.NullableTransportChannel;

    /// <inheritdoc/>
    public OperationLimits OperationLimits => Session.OperationLimits;

    /// <inheritdoc/>
    public int OperationTimeout
    {
        get => Session.OperationTimeout;
        set => Session.OperationTimeout = value;
    }

    /// <inheritdoc/>
    public int OutstandingRequestCount => Session.OutstandingRequestCount;

    /// <inheritdoc/>
    public StringCollection PreferredLocales => Session.PreferredLocales;

    /// <inheritdoc/>
    public DiagnosticsMasks ReturnDiagnostics
    {
        get => Session.ReturnDiagnostics;
        set => Session.ReturnDiagnostics = value;
    }

    /// <inheritdoc/>
    public StringTable ServerUris => Session.ServerUris;

    /// <inheritdoc/>
    public ISessionFactory SessionFactory => Session.SessionFactory;

    /// <inheritdoc/>
    public NodeId SessionId => Session.SessionId;

    /// <inheritdoc/>
    public string SessionName => Session.SessionName;

    /// <inheritdoc/>
    public double SessionTimeout => Session.SessionTimeout;

    /// <inheritdoc/>
    public int SubscriptionCount => Session.SubscriptionCount;

    /// <inheritdoc/>
    public IEnumerable<Subscription> Subscriptions => Session.Subscriptions;

    /// <inheritdoc/>
    public ISystemContext SystemContext => Session.SystemContext;

    /// <inheritdoc/>
    public bool TransferSubscriptionsOnReconnect
    {
        get => Session.TransferSubscriptionsOnReconnect;
        set => Session.TransferSubscriptionsOnReconnect = value;
    }

    /// <inheritdoc/>
    public ITransportChannel TransportChannel => Session.TransportChannel;

    /// <inheritdoc/>
    public ITypeTable TypeTree => Session.TypeTree;

    /// <inheritdoc/>
    public ResponseHeader ActivateSession(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, out byte[] serverNonce, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, out serverNonce, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<ActivateSessionResponse> ActivateSessionAsync(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, CancellationToken ct) => Session.ActivateSessionAsync(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, ct);

    /// <inheritdoc/>
    public ResponseHeader AddNodes(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, out AddNodesResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.AddNodes(requestHeader, nodesToAdd, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<AddNodesResponse> AddNodesAsync(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, CancellationToken ct) => Session.AddNodesAsync(requestHeader, nodesToAdd, ct);

    /// <inheritdoc/>
    public ResponseHeader AddReferences(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.AddReferences(requestHeader, referencesToAdd, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<AddReferencesResponse> AddReferencesAsync(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, CancellationToken ct) => Session.AddReferencesAsync(requestHeader, referencesToAdd, ct);

    /// <inheritdoc/>
    public bool AddSubscription(Subscription subscription) => Session.AddSubscription(subscription);

    /// <inheritdoc/>
    public bool ApplySessionConfiguration(SessionConfiguration sessionConfiguration) => Session.ApplySessionConfiguration(sessionConfiguration);

    /// <inheritdoc/>
    public void AttachChannel(ITransportChannel channel) => Session.AttachChannel(channel);

    /// <inheritdoc/>
    public IAsyncResult BeginActivateSession(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, AsyncCallback callback, object asyncState) => Session.BeginActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginAddNodes(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, AsyncCallback callback, object asyncState) => Session.BeginAddNodes(requestHeader, nodesToAdd, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginAddReferences(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, AsyncCallback callback, object asyncState) => Session.BeginAddReferences(requestHeader, referencesToAdd, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginBrowse(RequestHeader requestHeader, ViewDescription view, NodeId nodeToBrowse, uint maxResultsToReturn, BrowseDirection browseDirection, NodeId referenceTypeId, bool includeSubtypes, uint nodeClassMask, AsyncCallback callback, object asyncState) => Session.BeginBrowse(requestHeader, view, nodeToBrowse, maxResultsToReturn, browseDirection, referenceTypeId, includeSubtypes, nodeClassMask, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginBrowse(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, AsyncCallback callback, object asyncState) => Session.BeginBrowse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginBrowseNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, AsyncCallback callback, object asyncState) => Session.BeginBrowseNext(requestHeader, releaseContinuationPoint, continuationPoint, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginBrowseNext(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, AsyncCallback callback, object asyncState) => Session.BeginBrowseNext(requestHeader, releaseContinuationPoints, continuationPoints, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCall(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, AsyncCallback callback, object asyncState) => Session.BeginCall(requestHeader, methodsToCall, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCancel(RequestHeader requestHeader, uint requestHandle, AsyncCallback callback, object asyncState) => Session.BeginCancel(requestHeader, requestHandle, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCloseSession(RequestHeader requestHeader, bool deleteSubscriptions, AsyncCallback callback, object asyncState) => Session.BeginCloseSession(requestHeader, deleteSubscriptions, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCreateMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, AsyncCallback callback, object asyncState) => Session.BeginCreateMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCreateSession(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, AsyncCallback callback, object asyncState) => Session.BeginCreateSession(requestHeader, clientDescription, serverUri, endpointUrl, SessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginCreateSubscription(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, AsyncCallback callback, object asyncState) => Session.BeginCreateSubscription(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginDeleteMonitoredItems(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, AsyncCallback callback, object asyncState) => Session.BeginDeleteMonitoredItems(requestHeader, subscriptionId, monitoredItemIds, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginDeleteNodes(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, AsyncCallback callback, object asyncState) => Session.BeginDeleteNodes(requestHeader, nodesToDelete, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginDeleteReferences(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, AsyncCallback callback, object asyncState) => Session.BeginDeleteReferences(requestHeader, referencesToDelete, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginDeleteSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, AsyncCallback callback, object asyncState) => Session.BeginDeleteSubscriptions(requestHeader, subscriptionIds, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginHistoryRead(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, AsyncCallback callback, object asyncState) => Session.BeginHistoryRead(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginHistoryUpdate(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, AsyncCallback callback, object asyncState) => Session.BeginHistoryUpdate(requestHeader, historyUpdateDetails, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginModifyMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, AsyncCallback callback, object asyncState) => Session.BeginModifyMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginModifySubscription(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, AsyncCallback callback, object asyncState) => Session.BeginModifySubscription(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginPublish(int timeout) => Session.BeginPublish(timeout);

    /// <inheritdoc/>
    public IAsyncResult BeginPublish(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, AsyncCallback callback, object asyncState) => Session.BeginPublish(requestHeader, subscriptionAcknowledgements, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginQueryFirst(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, AsyncCallback callback, object asyncState) => Session.BeginQueryFirst(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginQueryNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, AsyncCallback callback, object asyncState) => Session.BeginQueryNext(requestHeader, releaseContinuationPoint, continuationPoint, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginRead(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, AsyncCallback callback, object asyncState) => Session.BeginRead(requestHeader, maxAge, timestampsToReturn, nodesToRead, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginRegisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToRegister, AsyncCallback callback, object asyncState) => Session.BeginRegisterNodes(requestHeader, nodesToRegister, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginRepublish(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, AsyncCallback callback, object asyncState) => Session.BeginRepublish(requestHeader, subscriptionId, retransmitSequenceNumber, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginSetMonitoringMode(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, AsyncCallback callback, object asyncState) => Session.BeginSetMonitoringMode(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginSetPublishingMode(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, AsyncCallback callback, object asyncState) => Session.BeginSetPublishingMode(requestHeader, publishingEnabled, subscriptionIds, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginSetTriggering(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, AsyncCallback callback, object asyncState) => Session.BeginSetTriggering(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginTransferSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, AsyncCallback callback, object asyncState) => Session.BeginTransferSubscriptions(requestHeader, subscriptionIds, sendInitialValues, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginTranslateBrowsePathsToNodeIds(RequestHeader requestHeader, BrowsePathCollection browsePaths, AsyncCallback callback, object asyncState) => Session.BeginTranslateBrowsePathsToNodeIds(requestHeader, browsePaths, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginUnregisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToUnregister, AsyncCallback callback, object asyncState) => Session.BeginUnregisterNodes(requestHeader, nodesToUnregister, callback, asyncState);

    /// <inheritdoc/>
    public IAsyncResult BeginWrite(RequestHeader requestHeader, WriteValueCollection nodesToWrite, AsyncCallback callback, object asyncState) => Session.BeginWrite(requestHeader, nodesToWrite, callback, asyncState);

    /// <inheritdoc/>
    public ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view, NodeId nodeToBrowse, uint maxResultsToReturn, BrowseDirection browseDirection, NodeId referenceTypeId, bool includeSubtypes, uint nodeClassMask, out byte[] continuationPoint, out ReferenceDescriptionCollection references) => Session.Browse(requestHeader, view, nodeToBrowse, maxResultsToReturn, browseDirection, referenceTypeId, includeSubtypes, nodeClassMask, out continuationPoint, out references);

    /// <inheritdoc/>
    public ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<BrowseResponse> BrowseAsync(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, CancellationToken ct) => Session.BrowseAsync(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, ct);

    /// <inheritdoc/>
    public ResponseHeader BrowseNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection references) => Session.BrowseNext(requestHeader, releaseContinuationPoint, continuationPoint, out revisedContinuationPoint, out references);

    /// <inheritdoc/>
    public ResponseHeader BrowseNext(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.BrowseNext(requestHeader, releaseContinuationPoints, continuationPoints, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<BrowseNextResponse> BrowseNextAsync(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, CancellationToken ct) => Session.BrowseNextAsync(requestHeader, releaseContinuationPoints, continuationPoints, ct);

    /// <inheritdoc/>
    public IList<object> Call(NodeId objectId, NodeId methodId, params object[] args) => Session.Call(objectId, methodId, args);

    /// <inheritdoc/>
    public ResponseHeader Call(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, out CallMethodResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.Call(requestHeader, methodsToCall, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<IList<object>> CallAsync(NodeId objectId, NodeId methodId, CancellationToken ct = default, params object[] args) => Session.CallAsync(objectId, methodId, ct, args);

    /// <inheritdoc/>
    public Task<CallResponse> CallAsync(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, CancellationToken ct) => Session.CallAsync(requestHeader, methodsToCall, ct);

    /// <inheritdoc/>
    public ResponseHeader Cancel(RequestHeader requestHeader, uint requestHandle, out uint cancelCount) => Session.Cancel(requestHeader, requestHandle, out cancelCount);

    /// <inheritdoc/>
    public Task<CancelResponse> CancelAsync(RequestHeader requestHeader, uint requestHandle, CancellationToken ct) => Session.CancelAsync(requestHeader, requestHandle, ct);

    /// <inheritdoc/>
    public void ChangePreferredLocales(StringCollection preferredLocales) => Session.ChangePreferredLocales(preferredLocales);

    /// <inheritdoc/>
    public StatusCode Close(int timeout) => Session.Close(timeout);

    /// <inheritdoc/>
    public StatusCode Close(bool closeChannel) => Session.Close(closeChannel);

    /// <inheritdoc/>
    public StatusCode Close(int timeout, bool closeChannel) => Session.Close(timeout, closeChannel);

    /// <inheritdoc/>
    public StatusCode Close() => Session.Close();

    /// <inheritdoc/>
    public Task<StatusCode> CloseAsync(bool closeChannel, CancellationToken ct = default) => Session.CloseAsync(closeChannel, ct);

    /// <inheritdoc/>
    public Task<StatusCode> CloseAsync(int timeout, CancellationToken ct = default) => Session.CloseAsync(timeout, ct);

    /// <inheritdoc/>
    public Task<StatusCode> CloseAsync(int timeout, bool closeChannel, CancellationToken ct = default) => Session.CloseAsync(timeout, closeChannel, ct);

    /// <inheritdoc/>
    public Task<StatusCode> CloseAsync(CancellationToken ct = default) => Session.CloseAsync(ct);

    /// <inheritdoc/>
    public ResponseHeader CloseSession(RequestHeader requestHeader, bool deleteSubscriptions) => Session.CloseSession(requestHeader, deleteSubscriptions);

    /// <inheritdoc/>
    public Task<CloseSessionResponse> CloseSessionAsync(RequestHeader requestHeader, bool deleteSubscriptions, CancellationToken ct) => Session.CloseSessionAsync(requestHeader, deleteSubscriptions, ct);

    /// <inheritdoc/>
    public ResponseHeader CreateMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, out MonitoredItemCreateResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.CreateMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<CreateMonitoredItemsResponse> CreateMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, CancellationToken ct) => Session.CreateMonitoredItemsAsync(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, ct);

    /// <inheritdoc/>
    public ResponseHeader CreateSession(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, out NodeId sessionId, out NodeId authenticationToken, out double revisedSessionTimeout, out byte[] serverNonce, out byte[] serverCertificate, out EndpointDescriptionCollection serverEndpoints, out SignedSoftwareCertificateCollection serverSoftwareCertificates, out SignatureData serverSignature, out uint maxRequestMessageSize) => Session.CreateSession(requestHeader, clientDescription, serverUri, endpointUrl, sessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, out sessionId, out authenticationToken, out revisedSessionTimeout, out serverNonce, out serverCertificate, out serverEndpoints, out serverSoftwareCertificates, out serverSignature, out maxRequestMessageSize);

    /// <inheritdoc/>
    public Task<CreateSessionResponse> CreateSessionAsync(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, CancellationToken ct) => Session.CreateSessionAsync(requestHeader, clientDescription, serverUri, endpointUrl, SessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, ct);

    /// <inheritdoc/>
    public ResponseHeader CreateSubscription(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, out uint subscriptionId, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount) => Session.CreateSubscription(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, out subscriptionId, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);

    /// <inheritdoc/>
    public Task<CreateSubscriptionResponse> CreateSubscriptionAsync(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, CancellationToken ct) => Session.CreateSubscriptionAsync(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, ct);

    /// <inheritdoc/>
    public ResponseHeader DeleteMonitoredItems(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.DeleteMonitoredItems(requestHeader, subscriptionId, monitoredItemIds, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<DeleteMonitoredItemsResponse> DeleteMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, CancellationToken ct) => Session.DeleteMonitoredItemsAsync(requestHeader, subscriptionId, monitoredItemIds, ct);

    /// <inheritdoc/>
    public ResponseHeader DeleteNodes(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.DeleteNodes(requestHeader, nodesToDelete, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<DeleteNodesResponse> DeleteNodesAsync(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, CancellationToken ct) => Session.DeleteNodesAsync(requestHeader, nodesToDelete, ct);

    /// <inheritdoc/>
    public ResponseHeader DeleteReferences(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.DeleteReferences(requestHeader, referencesToDelete, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<DeleteReferencesResponse> DeleteReferencesAsync(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, CancellationToken ct) => Session.DeleteReferencesAsync(requestHeader, referencesToDelete, ct);

    /// <inheritdoc/>
    public ResponseHeader DeleteSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.DeleteSubscriptions(requestHeader, subscriptionIds, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<DeleteSubscriptionsResponse> DeleteSubscriptionsAsync(RequestHeader requestHeader, UInt32Collection subscriptionIds, CancellationToken ct) => Session.DeleteSubscriptionsAsync(requestHeader, subscriptionIds, ct);

    /// <inheritdoc/>
    public void DetachChannel() => Session.DetachChannel();

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
    }

    /// <inheritdoc/>
    public ResponseHeader EndActivateSession(IAsyncResult result, out byte[] serverNonce, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndActivateSession(result, out serverNonce, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndAddNodes(IAsyncResult result, out AddNodesResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndAddNodes(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndAddReferences(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndAddReferences(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndBrowse(IAsyncResult result, out byte[] continuationPoint, out ReferenceDescriptionCollection references) => Session.EndBrowse(result, out continuationPoint, out references);

    /// <inheritdoc/>
    public ResponseHeader EndBrowse(IAsyncResult result, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndBrowse(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndBrowseNext(IAsyncResult result, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection references) => Session.EndBrowseNext(result, out revisedContinuationPoint, out references);

    /// <inheritdoc/>
    public ResponseHeader EndBrowseNext(IAsyncResult result, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndBrowseNext(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndCall(IAsyncResult result, out CallMethodResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndCall(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndCancel(IAsyncResult result, out uint cancelCount) => Session.EndCancel(result, out cancelCount);

    /// <inheritdoc/>
    public ResponseHeader EndCloseSession(IAsyncResult result) => Session.EndCloseSession(result);

    /// <inheritdoc/>
    public ResponseHeader EndCreateMonitoredItems(IAsyncResult result, out MonitoredItemCreateResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndCreateMonitoredItems(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndCreateSession(IAsyncResult result, out NodeId sessionId, out NodeId authenticationToken, out double revisedSessionTimeout, out byte[] serverNonce, out byte[] serverCertificate, out EndpointDescriptionCollection serverEndpoints, out SignedSoftwareCertificateCollection serverSoftwareCertificates, out SignatureData serverSignature, out uint maxRequestMessageSize) => Session.EndCreateSession(result, out sessionId, out authenticationToken, out revisedSessionTimeout, out serverNonce, out serverCertificate, out serverEndpoints, out serverSoftwareCertificates, out serverSignature, out maxRequestMessageSize);

    /// <inheritdoc/>
    public ResponseHeader EndCreateSubscription(IAsyncResult result, out uint subscriptionId, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount) => Session.EndCreateSubscription(result, out subscriptionId, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);

    /// <inheritdoc/>
    public ResponseHeader EndDeleteMonitoredItems(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndDeleteMonitoredItems(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndDeleteNodes(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndDeleteNodes(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndDeleteReferences(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndDeleteReferences(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndDeleteSubscriptions(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndDeleteSubscriptions(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndHistoryRead(IAsyncResult result, out HistoryReadResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndHistoryRead(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndHistoryUpdate(IAsyncResult result, out HistoryUpdateResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndHistoryUpdate(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndModifyMonitoredItems(IAsyncResult result, out MonitoredItemModifyResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndModifyMonitoredItems(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndModifySubscription(IAsyncResult result, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount) => Session.EndModifySubscription(result, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);

    /// <inheritdoc/>
    public ResponseHeader EndPublish(IAsyncResult result, out uint subscriptionId, out UInt32Collection availableSequenceNumbers, out bool moreNotifications, out NotificationMessage notificationMessage, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndPublish(result, out subscriptionId, out availableSequenceNumbers, out moreNotifications, out notificationMessage, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndQueryFirst(IAsyncResult result, out QueryDataSetCollection queryDataSets, out byte[] continuationPoint, out ParsingResultCollection parsingResults, out DiagnosticInfoCollection diagnosticInfos, out ContentFilterResult filterResult) => Session.EndQueryFirst(result, out queryDataSets, out continuationPoint, out parsingResults, out diagnosticInfos, out filterResult);

    /// <inheritdoc/>
    public ResponseHeader EndQueryNext(IAsyncResult result, out QueryDataSetCollection queryDataSets, out byte[] revisedContinuationPoint) => Session.EndQueryNext(result, out queryDataSets, out revisedContinuationPoint);

    /// <inheritdoc/>
    public ResponseHeader EndRead(IAsyncResult result, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndRead(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndRegisterNodes(IAsyncResult result, out NodeIdCollection registeredNodeIds) => Session.EndRegisterNodes(result, out registeredNodeIds);

    /// <inheritdoc/>
    public ResponseHeader EndRepublish(IAsyncResult result, out NotificationMessage notificationMessage) => Session.EndRepublish(result, out notificationMessage);

    /// <inheritdoc/>
    public ResponseHeader EndSetMonitoringMode(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndSetMonitoringMode(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndSetPublishingMode(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndSetPublishingMode(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndSetTriggering(IAsyncResult result, out StatusCodeCollection addResults, out DiagnosticInfoCollection addDiagnosticInfos, out StatusCodeCollection removeResults, out DiagnosticInfoCollection removeDiagnosticInfos) => Session.EndSetTriggering(result, out addResults, out addDiagnosticInfos, out removeResults, out removeDiagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndTransferSubscriptions(IAsyncResult result, out TransferResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndTransferSubscriptions(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndTranslateBrowsePathsToNodeIds(IAsyncResult result, out BrowsePathResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndTranslateBrowsePathsToNodeIds(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public ResponseHeader EndUnregisterNodes(IAsyncResult result) => Session.EndUnregisterNodes(result);

    /// <inheritdoc/>
    public ResponseHeader EndWrite(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.EndWrite(result, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public void FetchNamespaceTables() => Session.FetchNamespaceTables();

    /// <inheritdoc/>
    public Task FetchNamespaceTablesAsync(CancellationToken ct = default) => Session.FetchNamespaceTablesAsync(ct);

    /// <inheritdoc/>
    public ReferenceDescriptionCollection FetchReferences(NodeId nodeId) => Session.FetchReferences(nodeId);

    /// <inheritdoc/>
    public void FetchReferences(IList<NodeId> nodeIds, out IList<ReferenceDescriptionCollection> referenceDescriptions, out IList<ServiceResult> errors) => Session.FetchReferences(nodeIds, out referenceDescriptions, out errors);

    /// <inheritdoc/>
    public Task<ReferenceDescriptionCollection> FetchReferencesAsync(NodeId nodeId, CancellationToken ct) => Session.FetchReferencesAsync(nodeId, ct);

    /// <inheritdoc/>
    public Task<(IList<ReferenceDescriptionCollection>, IList<ServiceResult>)> FetchReferencesAsync(IList<NodeId> nodeIds, CancellationToken ct) => Session.FetchReferencesAsync(nodeIds, ct);

    /// <inheritdoc/>
    public void FetchTypeTree(ExpandedNodeId typeId) => Session.FetchTypeTree(typeId);

    /// <inheritdoc/>
    public void FetchTypeTree(ExpandedNodeIdCollection typeIds) => Session.FetchTypeTree(typeIds);

    /// <inheritdoc/>
    public Task FetchTypeTreeAsync(ExpandedNodeId typeId, CancellationToken ct = default) => Session.FetchTypeTreeAsync(typeId, ct);

    /// <inheritdoc/>
    public Task FetchTypeTreeAsync(ExpandedNodeIdCollection typeIds, CancellationToken ct = default) => Session.FetchTypeTreeAsync(typeIds, ct);

    /// <inheritdoc/>
    public void FindComponentIds(NodeId instanceId, IList<string> componentPaths, out NodeIdCollection componentIds, out List<ServiceResult> errors) => Session.FindComponentIds(instanceId, componentPaths, out componentIds, out errors);

    /// <inheritdoc/>
    public ReferenceDescription FindDataDescription(NodeId encodingId) => Session.FindDataDescription(encodingId);

    /// <inheritdoc/>
    public Task<DataDictionary> FindDataDictionary(NodeId descriptionId, CancellationToken ct = default) => Session.FindDataDictionary(descriptionId, ct);

    /// <inheritdoc/>
    public ResponseHeader HistoryRead(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, out HistoryReadResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.HistoryRead(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<HistoryReadResponse> HistoryReadAsync(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, CancellationToken ct) => Session.HistoryReadAsync(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, ct);

    /// <inheritdoc/>
    public ResponseHeader HistoryUpdate(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, out HistoryUpdateResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.HistoryUpdate(requestHeader, historyUpdateDetails, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<HistoryUpdateResponse> HistoryUpdateAsync(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, CancellationToken ct) => Session.HistoryUpdateAsync(requestHeader, historyUpdateDetails, ct);

    /// <inheritdoc/>
    public IEnumerable<Subscription> Load(Stream stream, bool transferSubscriptions = false, IEnumerable<Type>? knownTypes = null) => Session.Load(stream, transferSubscriptions, knownTypes);

    /// <inheritdoc/>
    public IEnumerable<Subscription> Load(string filePath, bool transferSubscriptions = false, IEnumerable<Type>? knownTypes = null) => Session.Load(filePath, transferSubscriptions, knownTypes);

    /// <inheritdoc/>
    public DataDictionary LoadDataDictionary(ReferenceDescription dictionaryNode, bool forceReload = false) => Session.LoadDataDictionary(dictionaryNode, forceReload);

    /// <inheritdoc/>
    public Task<Dictionary<NodeId, DataDictionary>> LoadDataTypeSystem(NodeId? dataTypeSystem = null, CancellationToken ct = default) => Session.LoadDataTypeSystem(dataTypeSystem, ct);

    /// <inheritdoc/>
    public ResponseHeader ModifyMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, out MonitoredItemModifyResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.ModifyMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<ModifyMonitoredItemsResponse> ModifyMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, CancellationToken ct) => Session.ModifyMonitoredItemsAsync(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, ct);

    /// <inheritdoc/>
    public ResponseHeader ModifySubscription(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount) => Session.ModifySubscription(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);

    /// <inheritdoc/>
    public Task<ModifySubscriptionResponse> ModifySubscriptionAsync(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, CancellationToken ct) => Session.ModifySubscriptionAsync(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, ct);

    /// <inheritdoc/>
    public uint NewRequestHandle() => Session.NewRequestHandle();

    /// <inheritdoc/>
    public void Open(string sessionName, IUserIdentity identity) => Session.Open(sessionName, identity);

    /// <inheritdoc/>
    public void Open(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales) => Session.Open(sessionName, sessionTimeout, identity, preferredLocales);

    /// <inheritdoc/>
    public void Open(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, bool checkDomain) => Session.Open(sessionName, sessionTimeout, identity, preferredLocales, checkDomain);

    /// <inheritdoc/>
    public Task OpenAsync(string sessionName, IUserIdentity identity, CancellationToken ct) => Session.OpenAsync(sessionName, identity, ct);

    /// <inheritdoc/>
    public Task OpenAsync(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, CancellationToken ct) => Session.OpenAsync(sessionName, sessionTimeout, identity, preferredLocales, ct);

    /// <inheritdoc/>
    public Task OpenAsync(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, bool checkDomain, CancellationToken ct) => Session.OpenAsync(sessionName, sessionTimeout, identity, preferredLocales, checkDomain, ct);

    /// <inheritdoc/>
    public ResponseHeader Publish(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, out uint subscriptionId, out UInt32Collection availableSequenceNumbers, out bool moreNotifications, out NotificationMessage notificationMessage, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.Publish(requestHeader, subscriptionAcknowledgements, out subscriptionId, out availableSequenceNumbers, out moreNotifications, out notificationMessage, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<PublishResponse> PublishAsync(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, CancellationToken ct) => Session.PublishAsync(requestHeader, subscriptionAcknowledgements, ct);

    /// <inheritdoc/>
    public ResponseHeader QueryFirst(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, out QueryDataSetCollection queryDataSets, out byte[] continuationPoint, out ParsingResultCollection parsingResults, out DiagnosticInfoCollection diagnosticInfos, out ContentFilterResult filterResult) => Session.QueryFirst(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, out queryDataSets, out continuationPoint, out parsingResults, out diagnosticInfos, out filterResult);

    /// <inheritdoc/>
    public Task<QueryFirstResponse> QueryFirstAsync(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, CancellationToken ct) => Session.QueryFirstAsync(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, ct);

    /// <inheritdoc/>
    public ResponseHeader QueryNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, out QueryDataSetCollection queryDataSets, out byte[] revisedContinuationPoint) => Session.QueryNext(requestHeader, releaseContinuationPoint, continuationPoint, out queryDataSets, out revisedContinuationPoint);

    /// <inheritdoc/>
    public Task<QueryNextResponse> QueryNextAsync(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, CancellationToken ct) => Session.QueryNextAsync(requestHeader, releaseContinuationPoint, continuationPoint, ct);

    /// <inheritdoc/>
    public bool ReactivateSubscriptions(SubscriptionCollection subscriptions, bool sendInitialValues) => Session.ReactivateSubscriptions(subscriptions, sendInitialValues);

    /// <inheritdoc/>
    public Task<bool> ReactivateSubscriptionsAsync(SubscriptionCollection subscriptions, bool sendInitialValues, CancellationToken ct = default) => Session.ReactivateSubscriptionsAsync(subscriptions, sendInitialValues, ct);

    /// <inheritdoc/>
    public ResponseHeader Read(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.Read(requestHeader, maxAge, timestampsToReturn, nodesToRead, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<ReadResponse> ReadAsync(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, CancellationToken ct) => Session.ReadAsync(requestHeader, maxAge, timestampsToReturn, nodesToRead, ct);

    /// <inheritdoc/>
    public ReferenceDescriptionCollection ReadAvailableEncodings(NodeId variableId) => Session.ReadAvailableEncodings(variableId);

    /// <inheritdoc/>
    public void ReadDisplayName(IList<NodeId> nodeIds, out IList<string> displayNames, out IList<ServiceResult> errors) => Session.ReadDisplayName(nodeIds, out displayNames, out errors);

    /// <inheritdoc/>
    public Node ReadNode(NodeId nodeId) => Session.ReadNode(nodeId);

    /// <inheritdoc/>
    public Node ReadNode(NodeId nodeId, NodeClass nodeClass, bool optionalAttributes = true) => Session.ReadNode(nodeId, nodeClass, optionalAttributes);

    /// <inheritdoc/>
    public Task<Node> ReadNodeAsync(NodeId nodeId, CancellationToken ct = default) => Session.ReadNodeAsync(nodeId, ct);

    /// <inheritdoc/>
    public Task<Node> ReadNodeAsync(NodeId nodeId, NodeClass nodeClass, bool optionalAttributes = true, CancellationToken ct = default) => Session.ReadNodeAsync(nodeId, nodeClass, optionalAttributes, ct);

    /// <inheritdoc/>
    public void ReadNodes(IList<NodeId> nodeIds, out IList<Node> nodeCollection, out IList<ServiceResult> errors, bool optionalAttributes = false) => Session.ReadNodes(nodeIds, out nodeCollection, out errors, optionalAttributes);

    /// <inheritdoc/>
    public void ReadNodes(IList<NodeId> nodeIds, NodeClass nodeClass, out IList<Node> nodeCollection, out IList<ServiceResult> errors, bool optionalAttributes = false) => Session.ReadNodes(nodeIds, nodeClass, out nodeCollection, out errors, optionalAttributes);

    /// <inheritdoc/>
    public Task<(IList<Node>, IList<ServiceResult>)> ReadNodesAsync(IList<NodeId> nodeIds, NodeClass nodeClass, bool optionalAttributes = false, CancellationToken ct = default) => Session.ReadNodesAsync(nodeIds, nodeClass, optionalAttributes, ct);

    /// <inheritdoc/>
    public Task<(IList<Node>, IList<ServiceResult>)> ReadNodesAsync(IList<NodeId> nodeIds, bool optionalAttributes = false, CancellationToken ct = default) => Session.ReadNodesAsync(nodeIds, optionalAttributes, ct);

    /// <inheritdoc/>
    public DataValue ReadValue(NodeId nodeId) => Session.ReadValue(nodeId);

    /// <inheritdoc/>
    public object ReadValue(NodeId nodeId, Type expectedType) => Session.ReadValue(nodeId, expectedType);

    /// <inheritdoc/>
    public Task<DataValue> ReadValueAsync(NodeId nodeId, CancellationToken ct = default) => Session.ReadValueAsync(nodeId, ct);

    /// <inheritdoc/>
    public void ReadValues(IList<NodeId> nodeIds, out DataValueCollection values, out IList<ServiceResult> errors) => Session.ReadValues(nodeIds, out values, out errors);

    /// <inheritdoc/>
    public void ReadValues(IList<NodeId> variableIds, IList<Type> expectedTypes, out List<object> values, out List<ServiceResult> errors) => Session.ReadValues(variableIds, expectedTypes, out values, out errors);

    /// <inheritdoc/>
    public Task<(DataValueCollection, IList<ServiceResult>)> ReadValuesAsync(IList<NodeId> nodeIds, CancellationToken ct = default) => Session.ReadValuesAsync(nodeIds, ct);

    /// <inheritdoc/>
    public void Reconnect() => Session.Reconnect();

    /// <inheritdoc/>
    public void Reconnect(ITransportWaitingConnection connection) => Session.Reconnect(connection);

    /// <inheritdoc/>
    public void Reconnect(ITransportChannel channel) => Session.Reconnect(channel);

    /// <inheritdoc/>
    public Task ReconnectAsync(CancellationToken ct = default) => Session.ReconnectAsync(ct);

    /// <inheritdoc/>
    public Task ReconnectAsync(ITransportWaitingConnection connection, CancellationToken ct = default) => Session.ReconnectAsync(connection, ct);

    /// <inheritdoc/>
    public Task ReconnectAsync(ITransportChannel channel, CancellationToken ct = default) => Session.ReconnectAsync(channel, ct);

    /// <inheritdoc/>
    public ResponseHeader RegisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToRegister, out NodeIdCollection registeredNodeIds) => Session.RegisterNodes(requestHeader, nodesToRegister, out registeredNodeIds);

    /// <inheritdoc/>
    public Task<RegisterNodesResponse> RegisterNodesAsync(RequestHeader requestHeader, NodeIdCollection nodesToRegister, CancellationToken ct) => Session.RegisterNodesAsync(requestHeader, nodesToRegister, ct);

    /// <inheritdoc/>
    public bool RemoveSubscription(Subscription subscription) => Session.RemoveSubscription(subscription);

    /// <inheritdoc/>
    public Task<bool> RemoveSubscriptionAsync(Subscription subscription, CancellationToken ct = default) => Session.RemoveSubscriptionAsync(subscription, ct);

    /// <inheritdoc/>
    public bool RemoveSubscriptions(IEnumerable<Subscription> subscriptions) => Session.RemoveSubscriptions(subscriptions);

    /// <inheritdoc/>
    public Task<bool> RemoveSubscriptionsAsync(IEnumerable<Subscription> subscriptions, CancellationToken ct = default) => Session.RemoveSubscriptionsAsync(subscriptions, ct);

    /// <inheritdoc/>
    public bool RemoveTransferredSubscription(Subscription subscription) => Session.RemoveTransferredSubscription(subscription);

    /// <inheritdoc/>
    public bool Republish(uint subscriptionId, uint sequenceNumber, out ServiceResult error) => Session.Republish(subscriptionId, sequenceNumber, out error);

    /// <inheritdoc/>
    public ResponseHeader Republish(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, out NotificationMessage notificationMessage) => Session.Republish(requestHeader, subscriptionId, retransmitSequenceNumber, out notificationMessage);

    /// <inheritdoc/>
    public Task<(bool, ServiceResult)> RepublishAsync(uint subscriptionId, uint sequenceNumber, CancellationToken ct = default) => Session.RepublishAsync(subscriptionId, sequenceNumber, ct);

    /// <inheritdoc/>
    public Task<RepublishResponse> RepublishAsync(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, CancellationToken ct) => Session.RepublishAsync(requestHeader, subscriptionId, retransmitSequenceNumber, ct);

    /// <inheritdoc/>
    public bool ResendData(IEnumerable<Subscription> subscriptions, out IList<ServiceResult> errors) => Session.ResendData(subscriptions, out errors);

    /// <inheritdoc/>
    public Task<(bool, IList<ServiceResult>)> ResendDataAsync(IEnumerable<Subscription> subscriptions, CancellationToken ct = default) => Session.ResendDataAsync(subscriptions, ct);

    /// <inheritdoc/>
    public void Save(string filePath, IEnumerable<Type>? knownTypes = null) => Session.Save(filePath, knownTypes);

    /// <inheritdoc/>
    public void Save(Stream stream, IEnumerable<Subscription> subscriptions, IEnumerable<Type>? knownTypes = null) => Session.Save(stream, subscriptions, knownTypes);

    /// <inheritdoc/>
    public void Save(string filePath, IEnumerable<Subscription> subscriptions, IEnumerable<Type>? knownTypes = null) => Session.Save(filePath, subscriptions, knownTypes);

    /// <inheritdoc/>
    public SessionConfiguration SaveSessionConfiguration(Stream? stream = null) => Session.SaveSessionConfiguration(stream);

    /// <inheritdoc/>
    public ResponseHeader SetMonitoringMode(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.SetMonitoringMode(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<SetMonitoringModeResponse> SetMonitoringModeAsync(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, CancellationToken ct) => Session.SetMonitoringModeAsync(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, ct);

    /// <inheritdoc/>
    public ResponseHeader SetPublishingMode(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.SetPublishingMode(requestHeader, publishingEnabled, subscriptionIds, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<SetPublishingModeResponse> SetPublishingModeAsync(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, CancellationToken ct) => Session.SetPublishingModeAsync(requestHeader, publishingEnabled, subscriptionIds, ct);

    /// <inheritdoc/>
    public ResponseHeader SetTriggering(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, out StatusCodeCollection addResults, out DiagnosticInfoCollection addDiagnosticInfos, out StatusCodeCollection removeResults, out DiagnosticInfoCollection removeDiagnosticInfos) => Session.SetTriggering(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, out addResults, out addDiagnosticInfos, out removeResults, out removeDiagnosticInfos);

    /// <inheritdoc/>
    public Task<SetTriggeringResponse> SetTriggeringAsync(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, CancellationToken ct) => Session.SetTriggeringAsync(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, ct);

    /// <inheritdoc/>
    public void StartPublishing(int timeout, bool fullQueue) => Session.StartPublishing(timeout, fullQueue);

    /// <inheritdoc/>
    public bool TransferSubscriptions(SubscriptionCollection subscriptions, bool sendInitialValues) => Session.TransferSubscriptions(subscriptions, sendInitialValues);

    /// <inheritdoc/>
    public ResponseHeader TransferSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, out TransferResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.TransferSubscriptions(requestHeader, subscriptionIds, sendInitialValues, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<bool> TransferSubscriptionsAsync(SubscriptionCollection subscriptions, bool sendInitialValues, CancellationToken ct = default) => Session.TransferSubscriptionsAsync(subscriptions, sendInitialValues, ct);

    /// <inheritdoc/>
    public Task<TransferSubscriptionsResponse> TransferSubscriptionsAsync(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, CancellationToken ct) => Session.TransferSubscriptionsAsync(requestHeader, subscriptionIds, sendInitialValues, ct);

    /// <inheritdoc/>
    public ResponseHeader TranslateBrowsePathsToNodeIds(RequestHeader requestHeader, BrowsePathCollection browsePaths, out BrowsePathResultCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.TranslateBrowsePathsToNodeIds(requestHeader, browsePaths, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<TranslateBrowsePathsToNodeIdsResponse> TranslateBrowsePathsToNodeIdsAsync(RequestHeader requestHeader, BrowsePathCollection browsePaths, CancellationToken ct) => Session.TranslateBrowsePathsToNodeIdsAsync(requestHeader, browsePaths, ct);

    /// <inheritdoc/>
    public ResponseHeader UnregisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToUnregister) => Session.UnregisterNodes(requestHeader, nodesToUnregister);

    /// <inheritdoc/>
    public Task<UnregisterNodesResponse> UnregisterNodesAsync(RequestHeader requestHeader, NodeIdCollection nodesToUnregister, CancellationToken ct) => Session.UnregisterNodesAsync(requestHeader, nodesToUnregister, ct);

    /// <inheritdoc/>
    public void UpdateSession(IUserIdentity identity, StringCollection preferredLocales) => Session.UpdateSession(identity, preferredLocales);

    /// <inheritdoc/>
    public ResponseHeader Write(RequestHeader requestHeader, WriteValueCollection nodesToWrite, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos) => Session.Write(requestHeader, nodesToWrite, out results, out diagnosticInfos);

    /// <inheritdoc/>
    public Task<WriteResponse> WriteAsync(RequestHeader requestHeader, WriteValueCollection nodesToWrite, CancellationToken ct) => Session.WriteAsync(requestHeader, nodesToWrite, ct);
}
