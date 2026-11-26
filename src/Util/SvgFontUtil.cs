using SkiaSharp;
using Svg.Skia;
using Svg.Skia.TypefaceProviders;

namespace Iface.Oik.SvgPlayground.Util;

public static class SvgFontUtil
{
    private static SKTypeface _defaultTypefaceRegular;
    private static SKTypeface _defaultTypefaceBold;
    private static SKTypeface _defaultTypefaceItalic;
    private static SKTypeface _defaultTypefaceBoldItalic;

    public static void SetupDefaultFont(string fontFamily = "Arial")
    {
        if (_defaultTypefaceRegular != null) // уже инициализировано
        {
            return;
        }

        _defaultTypefaceRegular = SKTypeface.FromFamilyName(
            fontFamily,
            SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright
        );

        _defaultTypefaceBold = SKTypeface.FromFamilyName(
            fontFamily,
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright
        );

        _defaultTypefaceItalic = SKTypeface.FromFamilyName(
            fontFamily,
            SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Italic
        );

        _defaultTypefaceBoldItalic = SKTypeface.FromFamilyName(
            fontFamily,
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Italic
        );
    }

    public static SKTypeface GetTypeface(bool isBold = false, bool isItalic = false)
    {
        if (isBold && isItalic)
        {
            return _defaultTypefaceBoldItalic;
        }

        if (isBold)
        {
            return _defaultTypefaceBold;
        }

        if (isItalic)
        {
            return _defaultTypefaceItalic;
        }

        return _defaultTypefaceRegular;
    }

    public static void RegisterSvgTypeface(SKSvg skSvg)
    {
        skSvg.Settings.TypefaceProviders!.Insert(0, new SvgTypefaceProvider());
    }
}

public class SvgTypefaceProvider : ITypefaceProvider
{
    public SKTypeface FromFamilyName(
        string fontFamily,
        SKFontStyleWeight fontWeight,
        SKFontStyleWidth fontWidth,
        SKFontStyleSlant fontStyle
    )
    {
        return SvgFontUtil.GetTypeface(
            fontWeight == SKFontStyleWeight.Bold,
            fontStyle == SKFontStyleSlant.Italic
        );
    }
}
