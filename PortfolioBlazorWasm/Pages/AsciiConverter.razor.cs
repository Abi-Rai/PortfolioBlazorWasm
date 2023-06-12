using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System.Text;

namespace PortfolioBlazorWasm.Pages;

public partial class AsciiConverter
{
    [Inject] public ILogger<AsciiConverter> Logger { get; set; }
    [Inject] public HttpClient Http { get; set; }

    private string _asciiArt;
    private string _imgSrc;
    private readonly string[] _asciiChars =
    {
        " ",
        ".",
        "-",
        ":",
        "*",
        "+",
        "=",
        "%",
        "@",
        "#",
        "#"
    };
    private readonly List<string> _acceptableFormats = new()
    {
        "png",
        "jpeg"
    };
    private int _width = 40;
    private bool _validFile = false;
    private bool _runningConversion = false;
    private bool _usingSampleFile = false;
    private IBrowserFile _file;
    private const string _sample_image_path = "images/AsciiConverter/bugs.png";

    private async Task LoadSampleImage()
    {
        string format = Path.GetExtension(_sample_image_path).TrimStart('.').ToLower();
        byte[] buffer = await Http.GetByteArrayAsync(_sample_image_path);
        string base64String = Convert.ToBase64String(buffer);
        _imgSrc = $"data:image/{format};base64,{base64String}";
        _validFile = true;
        _usingSampleFile = true;
    }

    private async Task LoadFiles(IBrowserFile browserFile)
    {
        try
        {
            string format = browserFile.ContentType.Split('/')[1];
            if (!_acceptableFormats.Contains(format))
                throw new InvalidDataException($"wrong file format, acceptable formats are:{string.Join(',', _acceptableFormats)}");
            byte[] buffer = new byte[browserFile.Size];
            await browserFile.OpenReadStream().ReadAsync(buffer);
            string base64String = Convert.ToBase64String(buffer);
            _imgSrc = $"data:image/{format};base64,{base64String}";
            _file = browserFile;
            _validFile = true;
        }
        catch (Exception ex)
        {
            _validFile = false;
            Logger.LogError("Problem loading file:{0} stacktrace:{1}", ex.Message, ex.StackTrace);
            throw;
        }
        finally
        {
            _usingSampleFile = false;
        }
    }

    private async Task RegenerateImage()
    {
        try
        {
            _runningConversion = true;
            using var imageStream = _usingSampleFile ? await Http.GetStreamAsync(_sample_image_path) : _file.OpenReadStream();
            using var image = await Image.LoadAsync<Rgba32>(imageStream);
            Image<Rgba32> resizedImage = await GetReSizedImageAsync(image, _width);
            _asciiArt = await ConvertToAsciiAsync(resizedImage);
        }
        catch (Exception ex)
        {
            Logger.LogError("Problem generating art:{0} stacktrace:{1}", ex.Message, ex.StackTrace);
            throw;
        }
        finally
        {
            _runningConversion = false;
        }
    }

    private async Task<Image<Rgba32>> GetReSizedImageAsync(Image<Rgba32> inputBitmap, int asciiWidth)
    {
        int asciiHeight = 0;
        //Calculate the new Height of the image from its width
        asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);
        // using NearestNeighbor resampler
        await Task.Run(() => inputBitmap.Mutate(x => x.Resize(new ResizeOptions { Size = new SixLabors.ImageSharp.Size(asciiWidth, asciiHeight), Sampler = KnownResamplers.NearestNeighbor })));
        return inputBitmap;
    }

    private async Task<string> ConvertToAsciiAsync(Image<Rgba32> image)
    {
        // Quantize the image
        var quantizer = new WuQuantizer();
        using var quantizedImage = image.Clone(ctx => ctx.Quantize(quantizer));
        // Generate ASCII art
        Boolean toggle = true;
        StringBuilder sb = new();
        for (int h = 0; h < quantizedImage.Height; h++)
        {
            for (int w = 0; w < quantizedImage.Width; w++)
            {
                var pixelColor = quantizedImage[w, h];
                int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                Rgba32 grayColor = new Rgba32((byte)red, (byte)green, (byte)blue);
                if (!toggle)
                {
                    int index = (grayColor.R * (_asciiChars.Length - 1)) / 255;
                    sb.Append(_asciiChars[index]);
                }
            }

            if (!toggle)
            {
                sb.AppendLine();
                toggle = true;
            }
            else
            {
                toggle = false;
            }

            await Task.Delay(1);
            _asciiArt = sb.ToString();
            await InvokeAsync(StateHasChanged);
        }

        return sb.ToString();
    }
}