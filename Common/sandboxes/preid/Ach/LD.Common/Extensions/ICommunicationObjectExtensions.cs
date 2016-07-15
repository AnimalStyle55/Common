using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace LD.Common.Extensions
{
    /// <summary>
    /// Extensions for WCF service clients
    /// </summary>
    public static class ICommunicationObjectExtensions
    {
        /// <summary>
        /// Remarkably, disposing of WCF services or any VS service reference is not straightforward.
        /// This extension allows for disposing of them safely
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