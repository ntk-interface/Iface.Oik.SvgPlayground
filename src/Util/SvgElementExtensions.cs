using System;
using System.Globalization;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Util
{
  public static class SvgElementExtensions
  {
    public static string GetProperty(this SvgElement element, string property)
    {
      switch (property.ToLower())
      {
        case "stroke":       return element.GetStroke();
        case "fill":         return element.GetFill();
        case "fill-opacity": return element.GetFillOpacity();
        case "display":      return element.GetDisplay();
        case "x1":           return element.GetX1();
        case "x2":           return element.GetX2();
        case "y1":           return element.GetY1();
        case "y2":           return element.GetY2();
        case "x":            return element.GetX();
        case "y":            return element.GetY();
        case "width":        return element.GetWidth();
        case "height":       return element.GetHeight();
        case "text":         return element.GetText();
        default:
        {
          Console.WriteLine($"Unknown property {property}");
          return null;
        }
      }
    }


    public static bool SetProperty(this SvgElement element, string property, string value)
    {
      switch (property.ToLower())
      {
        case "stroke":       return element.SetStroke(value);
        case "fill":         return element.SetFill(value);
        case "fill-opacity": return element.SetFillOpacity(value);
        case "display":      return element.SetDisplay(value);
        case "x1":           return element.SetX1(value);
        case "x2":           return element.SetX2(value);
        case "y1":           return element.SetY1(value);
        case "y2":           return element.SetY2(value);
        case "x":            return element.SetX(value);
        case "y":            return element.SetY(value);
        case "width":        return element.SetWidth(value);
        case "height":       return element.SetHeight(value);
        case "transform":    return element.SetTransform(value);
        case "text":         return element.SetText(value);
        default:
        {
          Console.WriteLine($"Unknown property {property}");
          return false;
        }
      }
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
        Console.WriteLine("Cannot get display from non-visual element");
        return null;
      }
      return visualElement.Display;
    }


    private static string GetX1(this SvgElement element)
    {
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return null;
      }
      return lineElement.StartX.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetX2(this SvgElement element)
    {
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return null;
      }
      return lineElement.EndX.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetY1(this SvgElement element)
    {
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return null;
      }
      return lineElement.StartY.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetY2(this SvgElement element)
    {
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return null;
      }
      return lineElement.EndY.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetX(this SvgElement element)
    {
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return null;
      }
      return rectElement.X.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetY(this SvgElement element)
    {
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return null;
      }
      return rectElement.Y.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetWidth(this SvgElement element)
    {
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return null;
      }
      return rectElement.Width.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetHeight(this SvgElement element)
    {
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return null;
      }
      return rectElement.Height.Value.ToString(CultureInfo.InvariantCulture);
    }


    private static string GetText(this SvgElement element)
    {
      if (!(element is SvgText textElement))
      {
        Console.WriteLine("Cannot get text from non-text");
        return null;
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
        Console.WriteLine("Invalid fill opacity");
        return false;
      }
      element.FillOpacity = floatValue;
      return true;
    }


    private static bool SetDisplay(this SvgElement element, string value)
    {
      if (!(element is SvgVisualElement visualElement))
      {
        Console.WriteLine("Not a visual element");
        return false;
      }
      visualElement.Display = value;
      return true;
    }


    private static bool SetX1(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid x1");
        return false;
      }
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return false;
      }
      lineElement.StartX = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetX2(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid x2");
        return false;
      }
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return false;
      }
      lineElement.EndX = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetY1(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid y1");
        return false;
      }
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return false;
      }
      lineElement.StartY = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetY2(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid y2");
        return false;
      }
      if (!(element is SvgLine lineElement))
      {
        Console.WriteLine("Not a line");
        return false;
      }
      lineElement.EndY = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetX(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid x");
        return false;
      }
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return false;
      }
      rectElement.X = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetY(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid y");
        return false;
      }
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return false;
      }
      rectElement.Y = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetWidth(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid width");
        return false;
      }
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return false;
      }
      rectElement.Width = new SvgUnit(SvgUnitType.Pixel, floatValue);
      return true;
    }


    private static bool SetHeight(this SvgElement element, string value)
    {
      if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
      {
        Console.WriteLine("Invalid height");
        return false;
      }
      if (!(element is SvgRectangle rectElement))
      {
        Console.WriteLine("Not a rectangle");
        return false;
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
          Console.WriteLine("Cannot set text to non-text element");
          return false;
      }
    }
  }
}