using System;
using System.Runtime.InteropServices;
using static Hyperledger.Indy.Utils.CallbackHelper;

namespace Hyperledger.Indy.MetricsApi
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Checks whether a metrics exists.
        /// </summary>
        /// <param name="command_handle">The handle for the command that will be passed to the callback.</param>
        /// <param name="wallet_handle">wallet handle (created by open_wallet).</param>
        /// <param name="their_did">encrypted DID</param>
        /// <param name="cb">The function that will be called when the asynchronous call is complete.</param>
        /// <returns>0 if the command was initiated successfully.  Any non-zero result indicates an error.</returns>
        [DllImport(Consts.NATIVE_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int indy_is_metrics_exists(int command_handle, int wallet_handle, string their_did, IsMetricsExistsCompletedDelegate cb);

        /// <summary>
        /// Delegate for metrics exists that indicates whether or not a metrics exists.
        /// </summary>
        /// <param name="xcommand_handle">The handle for the command that initiated the callback.</param>
        /// <param name="err">The outcome of execution of the command.</param>
        /// <param name="exists">Whether or not the metrics exists.</param>
        internal delegate void IsMetricsExistsCompletedDelegate(int xcommand_handle, int err, bool exists);

        /// <summary>
        /// Creates metrics.
        /// </summary>
        /// <param name="command_handle">The handle for the command that will be passed to the callback.</param>
        /// <param name="wallet_handle">wallet handle (created by open_wallet).</param>
        /// <param name="their_did">encrypted DID</param>
        /// <param name="my_did">encrypted DID</param>
        /// <param name="metadata">Optional: extra information for metrics</param>
        /// <param name="cb">The function that will be called when the asynchronous call is complete.</param>
        /// <returns>0 if the command was initiated successfully.  Any non-zero result indicates an error.</returns>
        [DllImport(Consts.NATIVE_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int indy_create_metrics(int command_handle, int wallet_handle, string their_did, string my_did, string metadata, IndyMethodCompletedDelegate cb);

        /// <summary>
        /// Get list of saved metrics.
        /// </summary>
        /// <param name="command_handle">The handle for the command that will be passed to the callback.</param>
        /// <param name="wallet_handle">wallet handle (created by open_wallet).</param>
        /// <param name="cb">The function that will be called when the asynchronous call is complete.</param>
        /// <returns>0 if the command was initiated successfully.  Any non-zero result indicates an error.</returns>
        [DllImport(Consts.NATIVE_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int indy_list_metrics(int command_handle, int wallet_handle, ListMetricsCompletedDelegate cb);

        /// <summary>
        /// Delegate for listing saved metrics.
        /// </summary>
        /// <param name="xcommand_handle">The handle for the command that initiated the callback.</param>
        /// <param name="err">The outcome of execution of the command.</param>
        /// <param name="list_metrics">list of saved metrics</param>
        internal delegate void ListMetricsCompletedDelegate(int xcommand_handle, int err, string list_metrics);

        /// <summary>
        /// Gets metrics information for specific their_did.
        /// </summary>
        /// <param name="command_handle">The handle for the command that will be passed to the callback.</param>
        /// <param name="wallet_handle">wallet handle (created by open_wallet).</param>
        /// <param name="their_did">encrypted DID</param>
        /// <param name="cb">The function that will be called when the asynchronous call is complete.</param>
        /// <returns>0 if the command was initiated successfully.  Any non-zero result indicates an error.</returns>
        [DllImport(Consts.NATIVE_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int indy_get_metrics(int command_handle, int wallet_handle, string their_did, GetMetricsCompletedDelegate cb);

        /// <summary>
        /// Delegate for getting a saved metrics.
        /// </summary>
        /// <param name="xcommand_handle">The handle for the command that initiated the callback.</param>
        /// <param name="err">The outcome of execution of the command.</param>
        /// <param name="metrics_info_json">did info associated with their did</param>
        internal delegate void GetMetricsCompletedDelegate(int xcommand_handle, int err, string metrics_info_json);

        /// <summary>
        /// Save some data in the Wallet for metrics associated with Did.
        /// </summary>
        /// <param name="command_handle">The handle for the command that will be passed to the callback.</param>
        /// <param name="wallet_handle">wallet handle (created by open_wallet).</param>
        /// <param name="their_did">encrypted DID</param>
        /// <param name="metadata">some extra information for metrics</param>
        /// <param name="cb">The function that will be called when the asynchronous call is complete.</param>
        /// <returns>0 if the command was initiated successfully.  Any non-zero result indicates an error.</returns>
        [DllImport(Consts.NATIVE_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int indy_set_metrics_metadata(int command_handle, int wallet_handle, string their_did, string metadata, IndyMethodCompletedDelegate cb);

    }
}
