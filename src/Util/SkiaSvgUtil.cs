using SkiaSharp;
using Svg;
using Svg.Skia;

namespace Iface.Oik.SvgPlayground.Util
{
  public static class SkiaSvgUtil
  {
    public static void PaintSvgDocumentToSkiaCanvas(SvgDocument svgDocument,
                                                    SKCanvas    canvas,
                                                    float       x,
                                                    float       y,
                                                    float       scale)
    {
      if (svgDocument == null) return;

      canvas.Clear();

      using (var skSvg = new SKSvg())
      {
        skSvg.FromSvgDocument(svgDocument);
        var matrix = SKMatrix.MakeIdentity();
        SKMatrix.Concat(ref matrix,
                        SKMatrix.MakeTranslation(x, y),
                        SKMatrix.MakeScale(scale, scale));
        var image = SKImage.FromPicture(skSvg.Picture,
                                        new SKSizeI((int) (skSvg.Picture.CullRect.Width  * scale),
                                                    (int) (skSvg.Picture.CullRect.Height * scale)),
                                        matrix);
        canvas.DrawImage(image, 0, 0);
      }
    }
  }
}