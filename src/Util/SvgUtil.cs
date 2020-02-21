using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Util
{
  public class SvgUtil
  {
    public static SvgColourServer CreateColor(Color color)
    {
      return new SvgColourServer(color);
    }


    public static SvgPaintServer CreateColor(string value)
    {
      Color color;
      try
      {
        color = (Color) new ColorConverter().ConvertFromString(value);
      }
      catch (Exception)
      {
        return SvgPaintServer.None;
      }
      return CreateColor(color);
    }


    public static SvgElement FindTitleElement(SvgElement root)
    {
      if (root.Children.Count == 0) return null;

      foreach (var element in root.Children)
      {
        if (element is SvgTitle)
        {
          return element;
        }
        var nestedElement = FindTitleElement(element);
        if (nestedElement != null)
        {
          return nestedElement;
        }
      }

      return null;
    }


    public static SvgElement FindDescriptionElement(SvgElement root)
    {
      if (root.Children.Count == 0) return null;

      foreach (var element in root.Children)
      {
        if (element is SvgDescription)
        {
          return element;
        }
        var nestedElement = FindDescriptionElement(element);
        if (nestedElement != null)
        {
          return nestedElement;
        }
      }

      return null;
    }


    public static bool SetElementProperty(SvgElement element, string property, string value)
    {
      if (element == null)
      {
        Console.WriteLine($"Not an element");
        return false;
      }

      switch (property.ToLower())
      {
        case "stroke":
          element.Stroke = CreateColor(value);
          return true;

        case "fill":
        {
          element.Fill = CreateColor(value);
          return true;
        }

        case "fill-opacity":
        {
          if (!float.TryParse(value, out var floatValue))
          {
            Console.WriteLine("Invalid fill opacity");
            return false;
          }
          element.FillOpacity = floatValue;
          return true;
        }

        case "display":
        {
          if (!(element is SvgVisualElement visualElement))
          {
            Console.WriteLine("Not a visual element");
            return false;
          }
          visualElement.Display = value;
          return true;
        }

        case "x1":
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

        case "x2":
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

        case "y1":
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

        case "y2":
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

        case "x":
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

        case "y":
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

        case "width":
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

        case "height":
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

        case "transform":
        {
          element.Transforms = new SvgTransformConverter().ConvertFrom(value) as SvgTransformCollection;
          return true;
        }

        case "text":
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
              Console.WriteLine("Cannot set text to not text");
              return false;
          }
        }

        default:
        {
          Console.WriteLine($"Unknown property {property}");
          return false;
        }
      }
    }


    public static string GetElementProperty(SvgElement element, string property)
    {
      if (element == null)
      {
        Console.WriteLine($"Not an element");
        return null;
      }

      switch (property.ToLower())
      {
        case "stroke":
        {
          return (element.Stroke as SvgColourServer)?.ToString();
        }

        case "fill":
        {
          return (element.Fill as SvgColourServer)?.ToString();
        }

        case "fill-opacity":
        {
          return element.FillOpacity.ToString(CultureInfo.InvariantCulture);
        }

        case "x1":
        {
          if (!(element is SvgLine lineElement))
          {
            Console.WriteLine("Not a line");
            return null;
          }
          return lineElement.StartX.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "x2":
        {
          if (!(element is SvgLine lineElement))
          {
            Console.WriteLine("Not a line");
            return null;
          }
          return lineElement.EndX.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "y1":
        {
          if (!(element is SvgLine lineElement))
          {
            Console.WriteLine("Not a line");
            return null;
          }
          return lineElement.StartY.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "y2":
        {
          if (!(element is SvgLine lineElement))
          {
            Console.WriteLine("Not a line");
            return null;
          }
          return lineElement.EndY.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "x":
        {
          if (!(element is SvgRectangle rectElement))
          {
            Console.WriteLine("Not a rectangle");
            return null;
          }
          return rectElement.X.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "y":
        {
          if (!(element is SvgRectangle rectElement))
          {
            Console.WriteLine("Not a rectangle");
            return null;
          }
          return rectElement.Y.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "width":
        {
          if (!(element is SvgRectangle rectElement))
          {
            Console.WriteLine("Not a rectangle");
            return null;
          }
          return rectElement.Width.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "height":
        {
          if (!(element is SvgRectangle rectElement))
          {
            Console.WriteLine("Not a rectangle");
            return null;
          }
          return rectElement.Height.Value.ToString(CultureInfo.InvariantCulture);
        }

        case "display":
        {
          if (!(element is SvgVisualElement visualElement))
          {
            Console.WriteLine("Cannot get display from not visual element");
            return null;
          }
          return visualElement.Display;
        }

        case "text":
        {
          if (!(element is SvgText textElement))
          {
            Console.WriteLine("Cannot get text from not text");
            return null;
          }
          return textElement.Text;
        }

        default:
        {
          Console.WriteLine($"Unknown property {property}");
          return null;
        }
      }
    }


    public static IDictionary<string, SvgElement> GetElementsDictionaryWithAttribute(SvgElement root,
                                                                                     string     attribute)
    {
      var foundElements = new Dictionary<string, SvgElement>();
      AddElementsWithAttribute(root, attribute, foundElements);
      return foundElements;
    }


    public static ICollection<SvgElement> GetElementsCollectionWithAttribute(SvgElement root,
                                                                             string     attribute)
    {
      var foundElements = new Collection<SvgElement>();
      AddElementsWithAttribute(root, attribute, foundElements);
      return foundElements;
    }


    private static void AddElementsWithAttribute(SvgElement                      root,
                                                 string                          attribute,
                                                 IDictionary<string, SvgElement> foundElements)
    {
      if (root.Children.Count == 0) return;

      foreach (var element in root.Children)
      {
        if (element.TryGetAttribute(attribute, out var oikId))
        {
          foundElements.Add(oikId, element);
        }
        AddElementsWithAttribute(element, attribute, foundElements);
      }
    }


    private static void AddElementsWithAttribute(SvgElement              root,
                                                 string                  attribute,
                                                 ICollection<SvgElement> foundElements)
    {
      if (root.Children.Count == 0) return;

      foreach (var element in root.Children)
      {
        if (element.HasNonEmptyCustomAttribute(attribute))
        {
          foundElements.Add(element);
        }
        AddElementsWithAttribute(element, attribute, foundElements);
      }
    }
  }
}