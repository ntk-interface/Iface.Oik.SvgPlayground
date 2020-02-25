using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using Iface.Oik.SvgPlayground.Model;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Util
{
  public static class SvgUtil
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