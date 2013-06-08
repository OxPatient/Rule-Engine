using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yea.Compression
{
    /// <summary>
    /// Defines the various compression types that are available
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// BitArray
        /// </summary>
        BitArray = 0,
        /// <summary>
        /// Deflate
        /// </summary>
        Deflate = 1,
        /// <summary>
        /// GZip
        /// </summary>
        GZip = 2,

        Default = BitArray,
    }
}
