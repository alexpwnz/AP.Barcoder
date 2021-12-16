namespace AP.Barcoder
{
    public interface IBarcode
    {
        #region Public Property

        string Content { get; }
        Bounds Bounds { get; }
        Metadata Metadata { get; }

        #endregion

        bool At(int x, int y);
    }
}