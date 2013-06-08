#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Net;

#endregion

namespace Yea.IO
{
    /// <summary>
    ///     Uri Extension methods
    /// </summary>
    public static class UriExtensions
    {
        #region Execute

        /// <summary>
        ///     opens the URL in a browser
        /// </summary>
        /// <param name="url">URL to execute</param>
        /// <returns>The process object created when opening the URL</returns>
        public static Process Execute(this Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            return Process.Start(url.ToString());
        }

        #endregion

        #region Read

        /// <summary>
        ///     Reads the text content of a URL
        /// </summary>
        /// <param name="url">Uri to read the content of</param>
        /// <param name="userName">User name used in network credentials</param>
        /// <param name="password">Password used in network credentials</param>
        /// <param name="domain">Domain to use in network credentials</param>
        /// <param name="authenticationType">Authentication type to use in network credentials</param>
        /// <returns>String representation of the content of the URL</returns>
        public static string Read(this Uri url, string userName = "", string password = "", string domain = "",
                                  string authenticationType = "")
        {
            if (url == null)
                throw new ArgumentNullException("url");
            using (var client = new WebClient())
            {
                using (var reader = new StreamReader(url.Read(client, userName, password, domain, authenticationType)))
                {
                    string contents = reader.ReadToEnd();
                    return contents;
                }
            }
        }

        /// <summary>
        ///     Reads the text content of a URL
        /// </summary>
        /// <param name="url">The Uri to read the content of</param>
        /// <param name="client">WebClient used to load the data</param>
        /// <param name="userName">User name used in network credentials</param>
        /// <param name="password">Password used in network credentials</param>
        /// <param name="domain">Domain to use in network credentials</param>
        /// <param name="authenticationType">Authentication type to use in network credentials</param>
        /// <returns>Stream containing the content of the URL</returns>
        public static Stream Read(this Uri url, WebClient client, string userName = "", string password = "",
                                  string domain = "", string authenticationType = "")
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                NetworkCredential credentials = !string.IsNullOrEmpty(domain)
                                                    ? new NetworkCredential(userName, password, domain)
                                                    : new NetworkCredential(userName, password);
                var cache = new CredentialCache();
                if (!string.IsNullOrEmpty(authenticationType))
                {
                    cache.Add(url, authenticationType, credentials);
                    client.Credentials = cache;
                }
                else
                {
                    client.Credentials = credentials;
                }
            }
            return client.OpenRead(url);
        }

        #endregion

        #region ReadBinary

        /// <summary>
        ///     Reads the content of a URL
        /// </summary>
        /// <param name="url">Uri to read the content of</param>
        /// <param name="userName">User name used in network credentials</param>
        /// <param name="password">Password used in network credentials</param>
        /// <param name="domain">Domain to use in network credentials</param>
        /// <param name="authenticationType">Authentication type to use in network credentials</param>
        /// <returns>Byte array representation of the content of the URL</returns>
        public static byte[] ReadBinary(this Uri url, string userName = "", string password = "", string domain = "",
                                        string authenticationType = "")
        {
            if (url == null)
                throw new ArgumentNullException("url");
            using (var client = new WebClient())
            {
                using (Stream reader = url.Read(client, userName, password, domain, authenticationType))
                {
                    using (var finalStream = new MemoryStream())
                    {
                        while (true)
                        {
                            var buffer = new byte[1024];
                            int count = reader.Read(buffer, 0, buffer.Length);
                            if (count == 0)
                                break;
                            finalStream.Write(buffer, 0, count);
                        }
                        byte[] returnValue = finalStream.ToArray();
                        return returnValue;
                    }
                }
            }
        }

        #endregion
    }
}