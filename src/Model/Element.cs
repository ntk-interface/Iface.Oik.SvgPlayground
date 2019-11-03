using System;
using System.Collections.Generic;
using Iface.Oik.SvgPlayground.MainWindow;
using Iface.Oik.SvgPlayground.Util;
using Jint;
using Jint.Parser;
using Svg;

namespace Iface.Oik.SvgPlayground.Model
{
  public class Element
  {
    private readonly MainWindowViewModel _owner;
    private readonly SvgVisualElement    _svgElement;

    private readonly string                          _script;
    private readonly Engine                          _scriptEngine;
    private readonly IDictionary<string, SvgElement> _elementsWithId;
    private readonly bool                            _isUpdatable;

    public bool IsInit { get; }


    private Element(MainWindowViewModel owner,
                    SvgVisualElement    svgElement)
    {
      _owner      = owner;
      _svgElement = svgElement;

      var scriptElement = SvgUtil.FindDescriptionElement(_svgElement);
      if (scriptElement == null)
      {
        return;
      }
      _script = scriptElement.Content;

      _elementsWithId = new Dictionary<string, SvgElement>(SvgUtil.GetElementsDictionaryWithAttribute(_svgElement,
                                                                                                      "oikid"));

      _scriptEngine = new Engine();

      _scriptEngine
        .SetValue("setElementProperty",     new Action<string, string, string>(SetElementProperty))
        .SetValue("getElementProperty",     new Func<string, string, string>(GetElementProperty))
        .SetValue("initTmStatus",           new Func<int, int, int, string, int>(_owner.InitTmStatus))
        .SetValue("initTmAnalog",           new Func<int, int, int, string, int>(_owner.InitTmAnalog))
        .SetValue("getTmStatusStatus",      new Func<int, int>(_owner.GetTmStatusStatus))
        .SetValue("isTmStatusUnreliable",   new Func<int, bool>(_owner.IsTmStatusUnreliable))
        .SetValue("isTmStatusMalfunction",  new Func<int, bool>(_owner.IsTmStatusMalfunction))
        .SetValue("isTmStatusIntermediate", new Func<int, bool>(_owner.IsTmStatusIntermediate))
        .SetValue("isTmStatusOn",           new Func<int, bool>(_owner.IsTmStatusOn))
        .SetValue("getTmAnalogValue",       new Func<int, string>(_owner.GetTmAnalogValue))
        .SetValue("isTmAnalogUnreliable",   new Func<int, bool>(_owner.IsTmAnalogUnreliable))
        .Execute(_script, new ParserOptions {Tolerant = true});

      _isUpdatable = _scriptEngine.Global.HasProperty("update");

      IsInit = true;
    }


    public static Element Create(MainWindowViewModel owner, SvgElement rootElement)
    {
      if (owner == null ||
          !(rootElement is SvgVisualElement visualRootElement))
      {
        return null;
      }

      var oikSvgElement = new Element(owner, visualRootElement);

      return (oikSvgElement.IsInit) ? oikSvgElement : null;
    }


    public void Update()
    {
      if (!_isUpdatable) return;

      _scriptEngine.Invoke("update");
    }


    private void SetElementProperty(string id, string property, string value)
    {
      if (!_elementsWithId.TryGetValue(id, out var element)) return;

      SvgUtil.SetElementProperty(element, property, value);
    }


    private string GetElementProperty(string id, string property)
    {
      if (!_elementsWithId.TryGetValue(id, out var element)) return null;

      return SvgUtil.GetElementProperty(element, property);
    }
  }
}