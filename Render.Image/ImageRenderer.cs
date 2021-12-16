﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AP.Barcoder.Renderers;

namespace AP.Barcoder.Renderer.Image
{
    public sealed class ImageRenderer : IRenderer
    {
        #region Public Property

        public int BarHeightFor1DBarcode
        {
            get => _barHeightFor1DBarcode;
            set
            {
                if (value > 1) _barHeightFor1DBarcode = value;
            }
        }

        public int PixelSize
        {
            get => _pixelSize;
            set
            {
                if (value > 0) _pixelSize = value;
            }
        }

        #endregion

        #region Private Property

        private int MarginXLeft { get; set; }
        private int MarginXRight { get; set; }
        private int MarginYTop { get; set; }
        private int MarginYBottom { get; set; }

        #endregion

        #region Field

        private int _barHeightFor1DBarcode;

        //private readonly PngEncoder _pngEncoder = new PngEncoder();
        private int _pixelSize;

        public bool DrawString;

        #endregion

        public ImageRenderer(int pixelSize = 10, int barHeightFor1DBarcode = 40)
        {
            if (pixelSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelSize), "Value must be larger than zero");
            if (barHeightFor1DBarcode <= 0)
                throw new ArgumentOutOfRangeException(nameof(barHeightFor1DBarcode), "Value must be larger than zero");
            _pixelSize = pixelSize;
            _barHeightFor1DBarcode = barHeightFor1DBarcode;
            MarginXLeft = MarginXRight = MarginYTop = MarginYBottom = 5;
            DrawString = false;
        }

        #region ${Implements Interface} Members

        public void Render(IBarcode barcode, Stream outputStream)
        {
            barcode = barcode ?? throw new ArgumentNullException(nameof(barcode));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
            if (barcode.Bounds.Y == 1)
                Render1D(barcode, outputStream);
            else if (barcode.Bounds.Y > 1)
                Render2D(barcode, outputStream);
            else
                throw new NotSupportedException($"Y value of {barcode.Bounds.Y} is invalid");
        }

        #endregion

        public void SetMargin(int marginXLeft, int marginYTop, int marginXRight, int marginYBottom)
        {
            MarginXLeft = Math.Abs(marginXLeft);
            MarginXRight = Math.Abs(marginXRight);
            MarginYTop = Math.Abs(marginYTop);
            MarginYBottom = Math.Abs(marginYBottom);
        }

        public void SetMargin(int marginX, int marginY)
        {
            SetMargin(marginX, marginY, marginX, marginY);
        }

        public void SetMargin(int margin)
        {
            SetMargin(margin, margin, margin, margin);
        }

        private void Render1D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + MarginXLeft + MarginXRight) * _pixelSize;
            int height = (_barHeightFor1DBarcode + MarginYTop + MarginYBottom + (DrawString ? 10 : 0)) * _pixelSize;

            using (Bitmap image = new Bitmap(width, height))
            {
                Graphics g = Graphics.FromImage(image);

                g.FillRectangle(Brushes.White,
                    new Rectangle(0, 0, width, height));

                for (int x = 0; x < barcode.Bounds.X; x++)
                {
                    if (!barcode.At(x, 0)) continue;
                    g.FillRectangle(Brushes.Black,
                        new Rectangle(
                            (MarginXLeft + x) * _pixelSize,
                            MarginYTop * _pixelSize,
                            _pixelSize,
                            _barHeightFor1DBarcode * _pixelSize));
                }

                if (DrawString)
                {
                    int k = 0;
                    Font font = new Font("Courier", _pixelSize * 7, FontStyle.Bold);
                    g.DrawString(barcode.Content, font, Brushes.Black,
                        new Point(
                            (int) ((MarginXLeft + (barcode.Bounds.X - barcode.Content.Length * 5.8) / 2) * _pixelSize),
                            (MarginYTop + _barHeightFor1DBarcode) * _pixelSize));
                }

                image.Save(outputStream, ImageFormat.Jpeg);
            }
        }

        private void Render2D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + MarginXLeft + MarginXRight) * _pixelSize;
            int height = (barcode.Bounds.Y + MarginYTop + MarginYBottom) * _pixelSize;

            using (Bitmap image = new Bitmap(width, height))
            {
                Graphics g = Graphics.FromImage(image);

                g.FillRectangle(Brushes.White,
                    new Rectangle(0, 0, width, height));

                for (int y = 0; y < barcode.Bounds.Y; y++)
                for (int x = 0; x < barcode.Bounds.X; x++)
                {
                    if (!barcode.At(x, y)) continue;
                    g.FillRectangle(Brushes.Black,
                        new Rectangle(
                            (MarginXLeft + x) * _pixelSize,
                            (MarginYTop + y) * _pixelSize,
                            _pixelSize,
                            _pixelSize));
                }

                image.Save(outputStream, ImageFormat.Jpeg);
            }
        }
    }
}