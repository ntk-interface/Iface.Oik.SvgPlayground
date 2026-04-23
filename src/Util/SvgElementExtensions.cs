using System;
using System.Globalization;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Util;

public static class SvgElementExtensions
{
  public static bool IsVisible(this SvgElement element)
  {
    if (string.Equals(element.Display.Trim(), "none", StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(element.Visibility.Trim(), "visible", StringComparison.OrdinalIgnoreCase))
    {
      return false;
    }

    if (element.Parent is SvgGroup parentGroup)
    {
      if (!IsVisible(parentGroup))
      {
        return false;
      }
    }

    return true;
  }
  
  
  public static string GetProperty(this SvgElement element, string property)
  {
    return property.ToLower() switch
           {
             "stroke"       => element.GetStroke(),
             "fill"         => element.GetFill(),
             "fill-opacity" => element.GetFillOpacity(),
             "display"      => element.GetDisplay(),
             "x1"           => element.GetX1(),
             "x2"           => element.GetX2(),
             "y1"           => element.GetY1(),
             "y2"           => element.GetY2(),
             "x"            => element.GetX(),
             "y"            => element.GetY(),
             "width"        => element.GetWidth(),
             "height"       => element.GetHeight(),
             "text"         => element.GetText(),
             _              => throw new Exception($"Unknown property {property}"),
           };
  }


  public static bool SetProperty(this SvgElement element, string property, string value)
  {
    return property.ToLower() switch
           {
             "stroke"       => element.SetStroke(value),
             "fill"         => element.SetFill(value),
             "fill-opacity" => element.SetFillOpacity(value),
             "display"      => element.SetDisplay(value),
             "x1"           => element.SetX1(value),
             "x2"           => element.SetX2(value),
             "y1"           => element.SetY1(value),
             "y2"           => element.SetY2(value),
             "x"            => element.SetX(value),
             "y"            => element.SetY(value),
             "width"        => element.SetWidth(value),
             "height"       => element.SetHeight(value),
             "transform"    => element.SetTransform(value),
             "text"         => element.SetText(value),
             _              => throw new Exception($"Unknown property {property}")
           };
  }
  
  
  private static string GetStroke(this SvgElement element)
  {
    return (element.Stroke as SvgColourServer)?.ToString();
  }


  private static string GetFill(this SvgElement element)
  {
    return (element.Fill as SvgColourServer)?.ToString();
  }


  private static string GetFillOpacity(this SvgElement element)
  {
    return element.FillOpacity.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetDisplay(this SvgElement element)
  {
    if (!(element is SvgVisualElement visualElement))
    {
      throw new Exception("Cannot get display from non-visual element");
    }
    return visualElement.Display;
  }


  private static string GetX1(this SvgElement element)
  {
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    return lineElement.StartX.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetX2(this SvgElement element)
  {
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    return lineElement.EndX.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetY1(this SvgElement element)
  {
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    return lineElement.StartY.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetY2(this SvgElement element)
  {
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    return lineElement.EndY.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetX(this SvgElement element)
  {
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    return rectElement.X.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetY(this SvgElement element)
  {
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    return rectElement.Y.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetWidth(this SvgElement element)
  {
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    return rectElement.Width.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetHeight(this SvgElement element)
  {
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    return rectElement.Height.Value.ToString(CultureInfo.InvariantCulture);
  }


  private static string GetText(this SvgElement element)
  {
    if (!(element is SvgText textElement))
    {
      throw new Exception("Cannot get text from non-text");
    }
    return textElement.Text;
  }


  private static bool SetStroke(this SvgElement element, string value)
  {
    element.Stroke = SvgUtil.CreateColor(value);
    return true;
  }


  private static bool SetFill(this SvgElement element, string value)
  {
    element.Fill = SvgUtil.CreateColor(value);
    return true;
  }


  private static bool SetFillOpacity(this SvgElement element, string value)
  {
    if (!float.TryParse(value, out var floatValue))
    {
      throw new Exception("Invalid fill opacity");
    }
    element.FillOpacity = floatValue;
    return true;
  }


  private static bool SetDisplay(this SvgElement element, string value)
  {
    if (!(element is SvgVisualElement visualElement))
    {
      throw new Exception("Not a visual element");
    }
    visualElement.Display = value;
    return true;
  }


  private static bool SetX1(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid x1");
    }
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    lineElement.StartX = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetX2(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid x2");
    }
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    lineElement.EndX = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetY1(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid y1");
    }
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    lineElement.StartY = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetY2(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid y2");
    }
    if (!(element is SvgLine lineElement))
    {
      throw new Exception("Not a line");
    }
    lineElement.EndY = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetX(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid x");
    }
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    rectElement.X = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetY(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid y");
    }
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    rectElement.Y = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetWidth(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid width");
    }
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    rectElement.Width = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetHeight(this SvgElement element, string value)
  {
    if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
    {
      throw new Exception("Invalid height");
    }
    if (!(element is SvgRectangle rectElement))
    {
      throw new Exception("Not a rectangle");
    }
    rectElement.Height = new SvgUnit(SvgUnitType.Pixel, floatValue);
    return true;
  }


  private static bool SetTransform(this SvgElement element, string value)
  {
    element.Transforms = new SvgTransformConverter().ConvertFrom(value) as SvgTransformCollection;
    return true;
  }


  private static bool SetText(this SvgElement element, string value)
  {
    switch (element)
    {
      case SvgText textElement:
        textElement.Text = value;
        return true;
      case SvgTextSpan textSpanElement:
        textSpanElement.Text = value;
        return true;
      default:
        throw new Exception("Cannot set text to non-text element");
    }
  }
}