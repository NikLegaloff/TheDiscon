using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.DomainModel;
using QRCoder;

namespace DiscontMD.BusinessLogic.Bus.Commands
{
    public class PrintPagesPackCommand : ICommand
    {
        public PrintPagesPackCommand(Guid packId)
        {
            PackId = packId;
        }

        public PrintPagesPackCommand()
        {
        }

        public Guid PackId { get; set; }
    }


    public class PrintPagesPackCommandHandler : AbstractCommandHandler<PrintPagesPackCommand>
    {
        public PrintPagesPackCommandHandler(Guid dtoId) : base(dtoId)
        {
        }

        public override async Task<bool> Process(PrintPagesPackCommand command)
        {
            var packId = command.PackId;
            var pack = await Registry.Current.Data.CardPacks.Find(packId);

            var basePath = Registry.Current.Env.ImagesPath + pack.NumBase + "\\";
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            
            var num = pack.NumBase * 100;
            var w = 2034;
            var h = 3193;
            int fileCounter = num;
            Font drawFont = new Font("Arial", 73,FontStyle.Regular,GraphicsUnit.Pixel);
            for (var page = 0; page < 10; page++)
                using (var bmp = new Bitmap(w, h))
                {
                    bmp.MakeTransparent();
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAlias;
                        
                        for (var y = 0; y < 5; y++)
                            for (var x = 0; x < 2; x++)
                            {
                                var ax = x * w / 2 + 670;
                                var ay = y * 638 + 525;
                                g.DrawString(num.ToString(),drawFont, Brushes.Black, ax,ay);
                                var generator = new PayloadGenerator.Url("http://diskont-md.com/c/" + num);
                                string payload = generator.ToString();
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                                QRCode qrCode = new QRCode(qrCodeData);
                                
                                ay -= 262;
                                ax += 20;
                                using (var qr = qrCode.GetGraphic(8, Color.Black, Color.Transparent,false))
                                {
                                    g.DrawImage(qr, ax,ay);
                                }


                                num++;
                            }
                        var parameters = new EncoderParameters(1);
                        var parameter = new EncoderParameter(Encoder.Quality, 91L );
                        parameters.Param[0] = parameter;
                        var codec = GetImageCodecInfo(ImageFormat.Png);
                        var filename = basePath + fileCounter + ".png";
                        if (File.Exists(filename)) File.Delete(filename);
                        bmp.SetResolution(300, 300);
                        bmp.Save(filename, codec, parameters);
                        fileCounter+=10;
                    }
                }
            string startPath = basePath.TrimEnd('\\');
            string zipPath = Registry.Current.Env.ImagesPath + pack.NumBase + ".zip";
            ZipFile.CreateFromDirectory(startPath, zipPath);
            Directory.Delete(basePath,true);
            pack.Prepared = true;
            await Registry.Current.Data.CardPacks.Save(pack);
            return true;
        }

        public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;

            return null;
        }


    }
}