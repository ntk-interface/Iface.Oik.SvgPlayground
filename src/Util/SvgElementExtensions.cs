using System;
using System.ComponentModel;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Util;

public static class SvgElementExtensions
{
  public static bool IsVisibleRecursive(this SvgElement element)
  {
    if (string.Equals(element.Display.Trim(), "none", StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(element.Visibility.Trim(), "visible", StringComparison.OrdinalIgnoreCase))
    {
      return false;
    }

    if (element.Parent is SvgGroup parentGroup && !IsVisibleRecursive(parentGroup))
    {
      return false;
    }

    return true;
  }
  
  public static string GetProperty(this SvgElement element, string property)
  {
    if (property.Equals("text", StringComparison.CurrentCultureIgnoreCase))
    {
      if (element is not SvgText textElement)
      {
        throw new Exception("Cannot get text from non-text");
      }
      return textElement.Text;
    }
    
    var descriptor = TypeDescriptor.GetProperties(element).Find(property.Replace("-", string.Empty), 
                                                                ignoreCase: true);
    if (descriptor != null)
    {
      var value = descriptor.GetValue(element);
      if (value == null)
      {
        return string.Empty;
      }
      return descriptor.Converter?.ConvertToInvariantString(value);
    }

    if (element.CustomAttributes.TryGetValue(property, out var customValue))
    {
      return customValue;
    }

    throw new Exception($"Unknown property {property}");
  }


  public static void SetProperty(this SvgElement element, string property, string value)
  {
    switch (property.ToLower())
    {
      case "text":
        if (element is not SvgTextBase textElement)
        {
          throw new Exception("Cannot set text to non-text element");
        }
        textElement.Text = value;
        return;
      
      case "transform":
        element.Transforms = (SvgTransformCollection)new SvgTransformConverter().ConvertFromInvariantString(value);
        return;
      
      default:
        var descriptor = TypeDescriptor.GetProperties(element).Find(property.Replace("-", string.Empty), 
                                                                    ignoreCase: true);
        
        if (descriptor != null && !descriptor.IsReadOnly)
        {
          var convertedValue = descriptor.Converter?.ConvertFromInvariantString(value);
          descriptor.SetValue(element, convertedValue);
        }
        else
        {
          element.CustomAttributes[property] = value;
        }
        return;
    }
  }
}