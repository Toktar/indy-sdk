using Hyperledger.Indy.Utils;
using Hyperledger.Indy.WalletApi;
using System.Threading.Tasks;
using static Hyperledger.Indy.MetricsApi.NativeMethods;
#if __IOS__
using ObjCRuntime;
#endif

namespace Hyperledger.Indy.MetricsApi
{
    /// <summary>
    /// Provides methods for managing metrics identifiers.
    /// </summary>
    /// <remarks>
    /// A Metrics is a record of the relationship between a DID owned by the caller of the API and
    /// one belonging to another party, referred to respectively in this API  as <c>myDID</c>and <c>theirDID</c>.
    /// Metrics records can also hold additional optional metadata.
    /// </remarks>
    public static class Metrics
    {
        /// <summary>
        /// Gets the callback to use when the IsExistsAsync command completes.
        /// </summary>
#if __IOS__
        [MonoPInvokeCallback(typeof(IsMetricsExistsCompletedDelegate))]
#endif
        private static void IsMetricsExistsCallbackMethod(int xcommand_handle, int err, bool exists)
        {
            var taskCompletionSource = PendingCommands.Remove<bool>(xcommand_handle);

            if (!CallbackHelper.CheckCallback(taskCompletionSource, err))
                return;

            taskCompletionSource.SetResult(exists);
        }
        private static IsMetricsExistsCompletedDelegate IsMetricsExistsCallback = IsMetricsExistsCallbackMethod;

        /// <summary>
        /// Gets the callback to use when the ListAsync command completes.
        /// </summary>
#if __IOS__
        [MonoPInvokeCallback(typeof(ListMetricsCompletedDelegate))]
#endif
        private static void ListMetricsCallbackMethod(int xcommand_handle, int err, string list_metrics)
        {
            var taskCompletionSource = PendingCommands.Remove<string>(xcommand_handle);

            if (!CallbackHelper.CheckCallback(taskCompletionSource, err))
                return;

            taskCompletionSource.SetResult(list_metrics);
        }
        private static ListMetricsCompletedDelegate ListMetricsCallback = ListMetricsCallbackMethod;

        /// <summary>
        /// Gets the callback to use when the GetAsync command completes.
        /// </summary>
#if __IOS__
        [MonoPInvokeCallback(typeof(GetMetricsCompletedDelegate))]
#endif
        private static void GetMetricsCallbackMethod(int xcommand_handle, int err, string get_metrics_json)
        {
            var taskCompletionSource = PendingCommands.Remove<string>(xcommand_handle);

            if (!CallbackHelper.CheckCallback(taskCompletionSource, err))
                return;

            taskCompletionSource.SetResult(get_metrics_json);
        }
        private static GetMetricsCompletedDelegate GetMetricsCallback = GetMetricsCallbackMethod;

        /// <summary>
        /// Gets whether or not a metrics record exists in the provided wallet for the specified DID .
        /// </summary>
        /// <param name="wallet">The wallet to check for a metrics record.</param>
        /// <param name="theirDid">The DID to check.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> that resolves to true if a metrics exists for the 
        /// DID, otherwise false.</returns>
        public static Task<bool> IsExistsAsync(Wallet wallet, string theirDid)
        {
            ParamGuard.NotNull(wallet, "wallet");
            ParamGuard.NotNullOrWhiteSpace(theirDid, "theirDid");

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var commandHandle = PendingCommands.Add(taskCompletionSource);

            int result = NativeMethods.indy_is_metrics_exists(
                commandHandle,
                wallet.Handle,
                theirDid,
                IsMetricsExistsCallback);

            CallbackHelper.CheckResult(result);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Creates a new metrics record between two specified DIDs in the provided wallet.
        /// </summary>
        /// <param name="wallet">The wallet to store create the metrics record in.</param>
        /// <param name="theirDid">The DID of the remote party.</param>
        /// <param name="myDid">The DID belonging to the owner of the wallet.</param>
        /// <param name="metadata">Optional metadata to store with the record.</param>
        /// <returns>An asynchronous <see cref="Task"/> completes once the operation completes.</returns>
        public static Task CreateAsync(Wallet wallet, string theirDid, string myDid, string metadata)
        {
            ParamGuard.NotNull(wallet, "wallet");
            ParamGuard.NotNullOrWhiteSpace(theirDid, "theirDid");
            ParamGuard.NotNullOrWhiteSpace(myDid, "myDid");

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var commandHandle = PendingCommands.Add(taskCompletionSource);

            int result = NativeMethods.indy_create_metrics(
                commandHandle,
                wallet.Handle,
                theirDid,
                myDid,
                metadata,
                CallbackHelper.TaskCompletingNoValueCallback);

            CallbackHelper.CheckResult(result);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Lists all metrics relationships stored in the specified wallet.
        /// </summary>
        /// <remarks>
        /// The JSON string that this method resolves to will contain a array of objects each of which
        /// describes a metrics record for two DIDs, a DID belonging to the record owner (my_did) and the 
        /// associated DID belonging to the other party (their_did).
        /// 
        /// <code>
        /// [
        ///     {"my_did":"my_did_for_A","their_did":"A's_did_for_me"},
        ///     {"my_did":"my_did_for_B","their_did":"B's_did_for_me"}
        ///     ...
        /// ]
        /// </code>
        /// 
        /// Note that this call does not return any metadata associated with the metrics records; to get the
        /// metadata use the <see cref="GetAsync(Wallet, string)"/> method.
        /// </remarks>
        /// <param name="wallet">The wallet to get the metrics records from.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> that resolves to a JSON string containing
        /// an array of all metrics relationships stored in the wallet.</returns>
        public static Task<string> ListAsync(Wallet wallet)
        {
            ParamGuard.NotNull(wallet, "wallet");

            var taskCompletionSource = new TaskCompletionSource<string>();
            var commandHandle = PendingCommands.Add(taskCompletionSource);

            int result = NativeMethods.indy_list_metrics(
                commandHandle,
                wallet.Handle,
                ListMetricsCallback);

            CallbackHelper.CheckResult(result);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Gets the metrics record associated with the specified DID from the provided wallet.
        /// </summary>
        /// <remarks>
        /// The JSON string that this method resolves to will contain a single metrics record for two DIDs, 
        /// the DID belonging to the record owner (my_did), the associated DID belonging to the other party 
        /// (their_did) and any metadata associated with the record (metadata).
        /// 
        /// <code>
        /// [
        ///     {"my_did":"my_did_for_A","their_did":"A's_did_for_me","metadata":"some metadata"},
        ///     {"my_did":"my_did_for_B","their_did":"B's_did_for_me"}
        ///     ...
        /// ]
        /// </code>
        /// 
        /// Note that if no metadata is present in a record the JSON will omit the <c>metadata</c>key.
        /// </remarks>
        /// <param name="wallet">The wallet to get the metrics record from.</param>
        /// <param name="theirDid">The DID belonging to another party to get the metrics record for.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> that resolves to a JSON string containing
        /// a metrics record.</returns>
        public static Task<string> GetAsync(Wallet wallet, string theirDid)
        {
            ParamGuard.NotNull(wallet, "wallet");
            ParamGuard.NotNullOrWhiteSpace(theirDid, "theirDid");

            var taskCompletionSource = new TaskCompletionSource<string>();
            var commandHandle = PendingCommands.Add(taskCompletionSource);

            int result = NativeMethods.indy_get_metrics(
                commandHandle,
                wallet.Handle,
                theirDid,
                GetMetricsCallback);

            CallbackHelper.CheckResult(result);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the metadata on the existing metrics record for the specified DID in the provided wallet.
        /// </summary>
        /// <remarks>
        /// If the metrics record already contains any existing metadata it will be replaced with the value provided 
        /// in the <paramref name="metadata"/> parameter.  To remove all metadata for a record provide <c>null</c> in the
        /// <paramref name="metadata"/> parameter.
        /// </remarks>
        /// <param name="wallet">The wallet containing the metrics record.</param>
        /// <param name="theirDid">The DID belonging to another party the metrics record exists for.</param>
        /// <param name="metadata">The metadata to set on the metrics record.</param>
        /// <returns>An asynchronous <see cref="Task"/> completes once the operation completes.</returns>
        public static Task SetMetadataAsync(Wallet wallet, string theirDid, string metadata)
        {
            ParamGuard.NotNull(wallet, "wallet");
            ParamGuard.NotNullOrWhiteSpace(theirDid, "theirDid");

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var commandHandle = PendingCommands.Add(taskCompletionSource);

            int result = NativeMethods.indy_set_metrics_metadata(
                commandHandle,
                wallet.Handle,
                theirDid,
                metadata,
                CallbackHelper.TaskCompletingNoValueCallback);

            CallbackHelper.CheckResult(result);

            return taskCompletionSource.Task;
        }
    }
}
