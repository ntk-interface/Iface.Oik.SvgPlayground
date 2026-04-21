using System.Drawing;
using SkiaSharp;
using Svg;
using Svg.Skia;

namespace Iface.Oik.SvgPlayground.Util;

public static class SkiaSvgUtil
{
  public static void PaintSvgDocumentToSkiaCanvas(SvgDocument svgDocument,
                                                  SKColor     bgColor,
                                                  SKCanvas    canvas,
                                                  float       x,
                                                  float       y,
                                                  float       scale)
  {
    if (svgDocument == null) return;

    canvas.Clear(bgColor);

    using (var skSvg = new SKSvg())
    {
      skSvg.FromSvgDocument(svgDocument);
      var matrix = SKMatrix.CreateIdentity();
      SKMatrix.Concat(ref matrix,
                      SKMatrix.CreateTranslation(x, y),
                      SKMatrix.CreateScale(scale, scale));
      var image = SKImage.FromPicture(skSvg.Picture,
                                      new SKSizeI((int) (skSvg.Picture.CullRect.Width  * scale),
                                                  (int) (skSvg.Picture.CullRect.Height * scale)),
                                      matrix);
      canvas.DrawImage(image, 0, 0);
    }
  }


  public static SKColor GetSkColor(Color? color)
  {
    if (!color.HasValue) return SKColor.Empty;

    return new SKColor(color.Value.R,
                       color.Value.G,
                       color.Value.B,
                       color.Value.A);
  }


  public static SKColor GetSkColor(string rgbString)
  {
    SKColor.TryParse(rgbString, out SKColor color);
    return color;
  }
}