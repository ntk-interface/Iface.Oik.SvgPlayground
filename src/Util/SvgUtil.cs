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
          element.Fill = CreateColor(value);
          return true;

        case "fill-opacity":
          if (!float.TryParse(value, out var floatValue))
          {
            Console.WriteLine("Invalid fill opacity");
            return false;
          }
          element.FillOpacity = floatValue;
          return true;

        case "display":
          if (!(element is SvgVisualElement visualElement))
          {
            Console.WriteLine("Not a visual element");
            return false;
          }
          visualElement.Display = value;
          return true;

        case "transform":
          element.Transforms = new SvgTransformConverter().ConvertFrom(value) as SvgTransformCollection;
          return true;

        case "text":
          if (!(element is SvgText textElement))
          {
            Console.WriteLine("Cannot set text to not text");
            return false;
          }
          textElement.Text = value;
          return true;

        default:
          Console.WriteLine($"Unknown property {property}");
          return false;
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
          return (element.Stroke as SvgColourServer)?.ToString();
        
        case "fill":
          return (element.Fill as SvgColourServer)?.ToString();

        case "fill-opacity":
          return element.FillOpacity.ToString(CultureInfo.InvariantCulture);

        case "display":
          if (!(element is SvgVisualElement visualElement))
          {
            Console.WriteLine("Cannot get display from not visual element");
            return null;
          }
          return visualElement.Display;

        case "text":
          if (!(element is SvgText textElement))
          {
            Console.WriteLine("Cannot get text from not text");
            return null;
          }
          return textElement.Text;

        default:
          Console.WriteLine($"Unknown property {property}");
          return null;
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