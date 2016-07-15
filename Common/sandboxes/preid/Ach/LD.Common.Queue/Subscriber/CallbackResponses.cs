using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Queue.Subscriber
{
    /// <summary>
    /// Response received by callback to indicate what rabbit should do with the message
    /// </summary>
    public enum CallbackResponse
    {
        /// <summary>Processing Succeeded</summary>
        Acknowledge,

        /// <summary>Processing Failed, Requeue and try again</summary>
        ReQueue,

        /// <summary>Message was not processed, but do not try again, possibly send to a DLQ</summary>
        Dead
    }

    /// <summary>
    /// Response class returned by extended callback
    /// </summary>
    public class ExtendedCallbackResponse
    {
        /// <summary>
        /// Construct with a CallbackResponse
        /// </summary>
        public ExtendedCallbackResponse(CallbackResponse response)
        {
            Response = response;
        }

        /// <summary>
        /// The response which controls the fate of the message
        /// </summary>
        public CallbackResponse Response { get; }

        /// <summary>
        /// If Response is Acknowledge, true here will cause the message to be republished as a new message
        /// The properties and body will come from the message parameter to the callback
        /// NOTE: The republished message will not have the original routing key
        /// </summary>
        public bool Republish { get; set; }

        /// <summary>
        /// If Response is Acknowledge, a value here will cause the message to be republished as a new message to the named exchange
        /// The routing key, properties, and body will come from the message parameter to the callback
        /// </summary>
        public string PublishToExchange { get; set; }

        /// <summary>
        /// If Response is Acknowledge, a value here will cause the message to be republished as a new message to the named queue
        /// The properties and body will come from the message parameter to the callback
        /// NOTE: The republished message will not have the original routing key
        /// </summary>
        public string PublishToQueue { get; set; }
    }
}