using AP.Barcoder.Utils;

namespace AP.Barcoder.DataMatrix
{
    public sealed class DataMatrixCode : IBarcode
    {
        #region Public Property

        public int MarginXLeft => 5;
        public int MarginXRight => 5;
        public int MarginYTop => 5;
        public int MarginYBottom => 5;

        #endregion

        #region Field

        private readonly BitList _data;
        private readonly CodeSize _size;

        #endregion

        internal DataMatrixCode(CodeSize size)
        {
            _size = size;
            Bounds = new Bounds(size.Columns, size.Rows);
            Metadata = new Metadata(BarcodeType.DataMatrix.GetStringValue(), 2);
            _data = new BitList(size.Rows * size.Columns);
        }

        #region ${Implements Interface} Members

        public string Content { get; internal set; }

        public Bounds Bounds { get; }

        public Metadata Metadata { get; }

        public bool At(int x, int y)
        {
            return Get(x, y);
        }

        #endregion

        internal void Set(int x, int y, bool value)
        {
            _data.SetBit(x * _size.Rows + y, value);
        }

        internal bool Get(int x, int y)
        {
            return _data.GetBit(x * _size.Rows + y);
        }
    }
}