namespace AP.Barcoder
{
    public interface IBarcodeIntCS : IBarcode
    {
        #region Public Property

        int Checksum { get; }

        #endregion
    }
}