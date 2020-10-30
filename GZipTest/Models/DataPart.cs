namespace GZipTest.Models
{
    /// <summary>
    /// Part of all data with starting and ending positions
    /// </summary>
    public sealed class DataPart
    {
        /// <summary>
        /// Part of all data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Starting position of data part in source
        /// </summary>
        public long StartPosition { get; set; }

        /// <summary>
        /// Ending position of data part in source
        /// </summary>
        public long EndPosition { get; set; }

        public DataPart()
        {

        }


        public DataPart(byte[] data, long startPosition, long endPosition)
        {
            this.Data = data;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }
    }
}
