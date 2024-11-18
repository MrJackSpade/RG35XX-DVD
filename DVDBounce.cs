using DVD.Extensions;
using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.GamePads;
using RG35XX.Libraries;
using SixLabors.ImageSharp;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text.Json;
using Color = RG35XX.Core.Drawing.Color;

namespace DVD
{
    public class DVDBounce
    {
        private readonly FrameBuffer _frameBuffer;

        private readonly GamePadReader _gamePadReader;

        private readonly HttpClient _httpClient;

        public DVDBounce()
        {
            HttpClientHandler handler = new()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            _httpClient = new HttpClient(handler);

            _gamePadReader = new GamePadReader();
            _gamePadReader.Initialize();

            _frameBuffer = new FrameBuffer();
            _frameBuffer.Initialize(640, 480);
        }

        public void Execute()
        {

            Thread bounceThread = new(this.Bounce);

            bounceThread.Start();

            _gamePadReader.ClearBuffer();

            do 
            {
                GamepadKey key = _gamePadReader.WaitForInput();

                if (key is GamepadKey.L1_DOWN)
                {
                    if (_delayMs == 0)
                    {
                        _delayMs = 1;
                    }
                    else
                    {
                        _delayMs *= 2;
                    }
                } else if ( key is GamepadKey.R1_DOWN)
                {
                    _delayMs /= 2;
                } else if (key is GamepadKey.MENU_DOWN or GamepadKey.B_DOWN)
                {
                    break;
                }
            } while (true);

            System.Environment.Exit(0);
        }

        private int _delayMs = 10;

        private void Bounce()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DVD.DVD.png";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);

            Image image = Image.Load(stream);

            Bitmap toDraw = new(image.Width + 2, image.Height + 2, Color.Black);

            toDraw.Draw(image.ToBitmap(), 1, 1);

            int x = 0;
            int y = 0;
            int xSpeed = 1;
            int ySpeed = 1;

            _frameBuffer.Clear();

            do
            {
                _frameBuffer.Draw(toDraw, x, y);

                x += xSpeed;
                y += ySpeed;

                if (x + toDraw.Width >= _frameBuffer.Width || x <= 0)
                {
                    xSpeed *= -1;
                }

                if (y + toDraw.Height >= _frameBuffer.Height || y <= 0)
                {
                    ySpeed *= -1;
                }

                System.Threading.Thread.Sleep(_delayMs);
            } while (true);
        }
    }
}