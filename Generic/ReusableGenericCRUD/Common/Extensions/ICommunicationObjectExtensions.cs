using System;
using System.ServiceModel;

namespace Common.Extensions
{
    /// <summary>
    /// Extensions for WCF service clients
    /// </summary>
    public static class ICommunicationObjectExtensions
    {
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/aa355056.aspx
        /// WCF Clients can throw in their close method, which makes a using block potentially swallow exceptions thrown within it
        /// This extension method safely closes the client
        /// </summary>
        /// <param name="client">service client stub</param>
        public static void Dispose(this ICommunicationObject client)
        {
            try
            {
                client.Close();
            }
            catch (CommunicationException)
            {
                // Abort if there is a communication exception.
                client.Abort();
            }
            catch (TimeoutException)
            {
                // Abort if there is a timeout exception.
                client.Abort();
            }
            catch (Exception)
            {
                // Abort in the face of any other exception.
                client.Abort();

                // Rethrow.
                throw;
            }
        }
    }
}