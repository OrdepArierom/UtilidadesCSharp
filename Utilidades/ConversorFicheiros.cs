using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Drawing; // Sistema.Drawing para .NET Framework
using System.Drawing.Imaging; // Sistema.Drawing.Imaging para .NET Framework
using System.IO;

namespace Utilidades
{
    public class ConversorFicheiros
    {
        public string ConverterTIFFParaPDF(string tiffBase64)
        {
            // Converte a base64 para um array de bytes
            byte[] tiffBytes = Convert.FromBase64String(tiffBase64);

            // Cria um MemoryStream a partir da array de bytes
            using (MemoryStream tiffStream = new MemoryStream(tiffBytes))
            {
                // Cria um documento PDF
                using (PdfDocument document = new PdfDocument())
                {
                    // Carrega o ficheiro TIFF a partir do seu MemoryStream
                    using (System.Drawing.Image tiffImage = System.Drawing.Image.FromStream(tiffStream))
                    {
                        // Obtém o número de frames do ficheiro TIFF
                        int frameCount = tiffImage.GetFrameCount(FrameDimension.Page);

                        for (int frameIdx = 0; frameIdx < frameCount; frameIdx++)
                        {
                            // Seleciona o frame atual
                            tiffImage.SelectActiveFrame(FrameDimension.Page, frameIdx);

                            // Converte o frame atual para um MemoryStream
                            using (MemoryStream frameStream = new MemoryStream())
                            {
                                tiffImage.Save(frameStream, ImageFormat.Png);
                                frameStream.Position = 0;

                                // Cria uma nova página PDF
                                PdfPage page = document.AddPage();
                                page.Width = tiffImage.Width;
                                page.Height = tiffImage.Height;

                                // Desenha a imagem TIFF na página PDF
                                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                                {
                                    using (XImage xImage = XImage.FromStream(frameStream))
                                    {
                                        gfx.DrawImage(xImage, 0, 0, tiffImage.Width, tiffImage.Height);
                                    }
                                }
                            }
                        }
                    }

                    // Cria um MemoryStream para guardar o documento PDF
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        // Guarda o documento PDF criado em um MemoryStream
                        document.Save(pdfStream);

                        // Converte o MemoryStream do documento PDF para um array de bytes
                        byte[] pdfBytes = pdfStream.ToArray();

                        // Converte o array de bytes do PDF para uma string base64
                        string pdfBase64 = Convert.ToBase64String(pdfBytes);

                        return pdfBase64;
                    }
                }
            }
        }
    }
}
