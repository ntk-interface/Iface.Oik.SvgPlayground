using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Iface.Oik.SvgPlayground.Model;
using Iface.Oik.SvgPlayground.Util;
using SkiaSharp;
using Svg;
using Svg.Skia;

namespace Iface.Oik.SvgPlayground.MainWindow;

public partial class MainWindowViewModel : ObservableObject
{
  private const float ScaleStep = 1.5f;
  private const float MoveStep  = 50f;

  private readonly MainWindowView _view;

  private Uri _svgUri;
  private SKSvg  _svg;
  
  
  private readonly List<Element> _elements = new();

  [ObservableProperty] private string _title;

  public int     Width   { get; private set; }
  public int     Height  { get; private set; }
  public SKColor BgColor { get; private set; }

  private float _scale = 1;
  private float _x     = 0;
  private float _y     = 0;


  public ObservableCollection<TmStatus> TmStatuses { get; } = new();
  public ObservableCollection<TmAnalog> TmAnalogs  { get; } = new();
  public ObservableCollection<Variable> Variables  { get; } = new();


  public ObservableCollection<LogItem> Log { get; } = new();


  private readonly bool _isBenchmarkingEnabled;


  private int           _tickInterval = 1000;
  private PeriodicTimer _tickTimer;


  public MainWindowViewModel(MainWindowView view)
  {
    _view = view;

    Title = "SVG";

    FixTextBoxFloatValue();

    _isBenchmarkingEnabled = Environment.GetCommandLineArgs().Contains("/benchmark");
  }


  private static void FixTextBoxFloatValue()
  {
    // FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
  }


  [RelayCommand]
  private async Task OpenFile()
  {
    var files = await TopLevel.GetTopLevel(_view)!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    {
      AllowMultiple  = false,
      FileTypeFilter = new[] { new FilePickerFileType("SVG") { Patterns = new[] { "*.svg" } } }
    });
    
    if (files.Count == 0)
    {
      return;
    }

    ExecuteOpenFile(files[0].Path);
  }


  [RelayCommand]
  private void ReloadFile()
  {
    ExecuteOpenFile(_svgUri);
  }


  private void ExecuteOpenFile(Uri uri)
  {
    _svgUri = uri;

    try
    {
      ClearEverything();

      var sw = new Stopwatch();
      if (_isBenchmarkingEnabled)
      {
        sw.Start();
      }
      _svg = new SKSvg();
      _svg.Load(uri.LocalPath);

      if (_isBenchmarkingEnabled)
      {
        AddToLog($"Загрузка файла SVG {sw.ElapsedMilliseconds}мс");
        sw.Restart();
      }

      SetBaseInfo();
      ParseElements();
      if (_isBenchmarkingEnabled)
      {
        AddToLog($"Определение активных элементов {sw.ElapsedMilliseconds}мс");
      }
      Update(updateElements: true);
      
      RunTickTimerIfNeeded();
    }
    catch (Exception ex)
    {
      _svg?.Dispose();
      _svg = null;
      AddToLog($"Ошибка при открытии файла: {ex.Message}");
    }
  }


  private void ClearEverything()
  {
    TmStatuses.Clear();
    TmAnalogs.Clear();
    Variables.Clear();
    _elements.Clear();

    ClearLog();

    _tickTimer?.Dispose();
  }


  private void SetBaseInfo()
  {
    Width  = (int)_svg.Picture!.CullRect.Width;
    Height = (int)_svg.Picture!.CullRect.Height;
    Title  = _svg.SourceDocument!.Descendants().OfType<SvgTitle>().FirstOrDefault()?.Content ?? "SVG";

    BgColor = SKColors.White;

    foreach (var node in _svg.SourceDocument!.Children)
    {
      switch (node)
      {
        case SvgRectangle rect when rect.Width is { Type : SvgUnitType.Percentage, Value: 100 } &&
                                    rect.Height is { Type: SvgUnitType.Percentage, Value: 100 } &&
                                    rect.Fill is SvgColourServer svgColourServer:
          BgColor = (uint) svgColourServer.Colour.ToArgb();
          break;

        case NonSvgElement nonSvg when nonSvg.Name == "namedview": // фон страницы в Inkscape
          if (nonSvg.CustomAttributes.TryGetValue("pagecolor", out var pageColorStr) &&
              SKColor.TryParse(pageColorStr, out var pageColor))
          {
            BgColor = pageColor;
          }
          break;
      }
    }
  }


  private void ParseElements()
  {
    _elements.Clear();
    
    try
    {
      if (_svg.TryEnsureRetainedSceneGraph(out var svgSceneDocument) && svgSceneDocument != null)
      {
        foreach (var svgNode in GetOikElementsSceneNodesRecursive(svgSceneDocument.Root))
        {
          var element = Element.Create(this, svgNode);
          if (element != null)
          {
            _elements.Add(element);
          }
        }
      }
    }
    catch (Exception ex)
    {
      _elements.Clear();
      AddToLog($"Ошибка при разборе SVG, схема не будет оживлена {ex.Message}");
    }
  }


  private IEnumerable<SvgSceneNode> GetOikElementsSceneNodesRecursive(SvgSceneNode node)
  {
    foreach (var childNode in node.Children)
    {
      foreach (var oikElementChildNode in GetOikElementsSceneNodesRecursive(childNode))
      {
        yield return oikElementChildNode;
      }
    }
    
    if (node.Element is SvgGroup groupElement && 
        groupElement.CustomAttributes.Keys.Any(k => k.EndsWith("oikelement")))
    {
      yield return node;
    }
  }


  private void Update(bool updateElements = false)
  {
    if (_svg == null)
    {
      return;
    }

    if (updateElements)
    {
      var sw = new Stopwatch();
      if (_isBenchmarkingEnabled)
      {
        sw.Start();
      }

      foreach (var element in _elements)
      {
        element.Update();
      }

      if (_isBenchmarkingEnabled)
      {
        AddToLog($"Элементы обновлены {sw.ElapsedMilliseconds}мс");
      }
    }
    

    InvalidateCanvas(updateElements);
  }


  private async void RunTickTimerIfNeeded()
  {
    if (!_elements.Any(element => element.IsTickEnabled))
    {
      return;
    }

    _tickTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_tickInterval));
    try
    {
      while (await _tickTimer.WaitForNextTickAsync())
      {
        InvokeScriptTick();
      }
    }
    catch (Exception ex)
    {
      AddToLog(ex.Message);
    }
  }


  private void InvokeScriptTick()
  {
    foreach (var element in _elements)
    {
      element.InvokeTick();
    }

    InvalidateCanvas(areElementsUpdated: true);
  }


  private void InvalidateCanvas(bool areElementsUpdated)
  {
    if (areElementsUpdated)
    {
      var sw = new Stopwatch();
      if (_isBenchmarkingEnabled)
      {
        sw.Start();
      }

      _svg.FromSvgDocument(_svg.SourceDocument);
    
      if (_isBenchmarkingEnabled)
      {
        AddToLog($"Рендер SVG в память {sw.ElapsedMilliseconds}мс");
      }
    }
    
    Dispatcher.UIThread.Post(() => _view?.InvalidateCanvas());
  }


  public void OnCanvasPaintSurface(SKCanvas canvas)
  {
    try
    {
      if (_svg?.Picture == null)
      {
        return;
      }
      var sw = new Stopwatch();
      if (_isBenchmarkingEnabled)
      {
        sw.Start();
      }
      canvas.Clear(BgColor);
      canvas.Translate(_x, _y);
      canvas.Scale(_scale);
      canvas.DrawPicture(_svg.Picture);
      if (_isBenchmarkingEnabled)
      {
        AddToLog($"Рендер SVG на экран {sw.ElapsedMilliseconds}мс");
      }
    }
    catch (Exception ex)
    {
      AddToLog($"Ошибка при попытке отрисовки: {ex.Message}");
    }
  }


  public void OnClick(float x, float y)
  {
    var element = FindElementUnderCursor(CorrectedX(x), CorrectedY(y));

    if (element == null)
    {
      return;
    }

    if (!string.IsNullOrEmpty(element.CustomToolTip))
    {
      AddToLog($"Вспл. подсказка \"{element.CustomToolTip}\"");
    }

    if (element.ClickCommand != null)
    {
      AddToLog(GetCommandDescription(element.ClickCommand));
    }

    foreach (var command in element.ContextMenuCommands.Where(cmd => cmd.Type != ElementCommandType.None))
    {
      AddToLog(GetCommandDescription(command));
    }
  }


  private string GetCommandDescription(ElementCommand command)
  {
    var result = $"{command.Level.GetDescription()}   {command.Type.GetDescription()}";

    switch (command.Type)
    {
      case ElementCommandType.ShowTmStatus:
      case ElementCommandType.Telecontrol:
      case ElementCommandType.SwitchTmStatusManually:
      case ElementCommandType.AckTmStatus:
      case ElementCommandType.OpenTmStatusEventsArchive:
      case ElementCommandType.CopyTmStatusToClipboard:
      case ElementCommandType.AddTmStatusToQuickList:
      case ElementCommandType.OpenTmStatusReport:
      case ElementCommandType.CopyTmStatusNameToClipboard:
      case ElementCommandType.OpenTmStatusChart:
        result += $"   {GetTmStatus(command.ParameterInt)}";
        break;
      
      case ElementCommandType.ShowTmAnalog:
      case ElementCommandType.Teleregulation:
      case ElementCommandType.SetTmAnalogManually:
      case ElementCommandType.OpenTmAnalogTechProperties:
      case ElementCommandType.OpenTmAnalogAlarms:
      case ElementCommandType.OpenTmAnalogChart:
      case ElementCommandType.OpenTmAnalogEventsArchive:
      case ElementCommandType.CopyTmAnalogToClipboard:
      case ElementCommandType.AddTmAnalogToQuickList:
      case ElementCommandType.OpenTmAnalogReport:
      case ElementCommandType.CopyTmAnalogNameToClipboard:
        result += $"   {GetTmAnalog(command.ParameterInt)}";
        break;
      
      case ElementCommandType.OpenDocumentInThisTab:
      case ElementCommandType.OpenDocumentInNewTab:
      case ElementCommandType.OpenDocumentInUniqueTab:
      case ElementCommandType.OpenDocumentInOverview:
      case ElementCommandType.StartProcess:
      case ElementCommandType.OpenVideoInOverview:
        result += $"   {command.ParameterString}";
        break;
    }

    return result;
  }


  [RelayCommand]
  private void ZoomIn()
  {
    Zoom(ScaleStep);
  }


  [RelayCommand]
  private void ZoomOut()
  {
    Zoom(1 / ScaleStep);
  }


  [RelayCommand]
  private void Zoom1()
  {
    Zoom(1 / _scale);
  }


  private void Zoom(float scale)
  {
    var tempScale = _scale * scale;
    if (tempScale < 1.1f && tempScale > 0.9f && !tempScale.Equals(1.0f)) // округляем до единицы, если рядом
    {
      scale /= tempScale;
    }

    _scale *= scale;

    Update();
  }


  [RelayCommand]
  private void MoveUp()
  {
    Move(0, -MoveStep);
  }


  [RelayCommand]
  private void MoveRight()
  {
    Move(MoveStep, 0);
  }


  [RelayCommand]
  private void MoveDown()
  {
    Move(0, +MoveStep);
  }


  [RelayCommand]
  private void MoveLeft()
  {
    Move(-MoveStep, 0);
  }


  private void Move(float x, float y)
  {
    _x += x;
    _y += y;

    Update();
  }


  private Element FindElementUnderCursor(float x, float y)
  {
    return _elements.FirstOrDefault(el => el.BoundContains(x, y) && el.IsVisible());
  }


  private float CorrectedX(float x)
  {
    return (x - _x) / _scale;
  }


  private float CorrectedY(float y)
  {
    return (y - _y) / _scale;
  }


  private int FindTmStatus(int ch, int rtu, int point)
  {
    var index = 0;
    foreach (var tmStatus in TmStatuses)
    {
      if (tmStatus.AddrEquals(ch, rtu, point))
      {
        return index;
      }
      index++;
    }
    return -1;
  }


  private int FindTmAnalog(int ch, int rtu, int point)
  {
    var index = 0;
    foreach (var tmAnalog in TmAnalogs)
    {
      if (tmAnalog.AddrEquals(ch, rtu, point))
      {
        return index;
      }
      index++;
    }
    return -1;
  }


  private int FindVariable(string id)
  {
    var index = 0;
    foreach (var variable in Variables)
    {
      if (variable.Id == id)
      {
        return index;
      }
      index++;
    }
    return -1;
  }


  public void OverrideUpdateInterval(int interval)
  {
    AddToLog($"Установлен интервал обновления данных: {interval}мс");
  }


  public void OverrideTickInterval(int interval)
  {
    _tickInterval = interval;

    AddToLog($"Установлен интервал циклического обновления tick: {interval}мс");
  }


  public int InitVariable(string id, string _ = null)
  {
    var existingVariable = FindVariable(id);
    if (existingVariable >= 0)
    {
      return existingVariable;
    }

    var variable = new Variable(id);
    Variables.Add(variable);

    return Variables.Count - 1;
  }


  public int InitTmStatus(int ch, int rtu, int point, string _ = null)
  {
    var existingTmStatusIndex = FindTmStatus(ch, rtu, point);
    if (existingTmStatusIndex >= 0)
    {
      return existingTmStatusIndex;
    }

    var tmStatus = new TmStatus(ch, rtu, point);

    TmStatuses.Add(tmStatus);
    
    tmStatus.PropertyChanged += (_, _) => Update(updateElements: true);

    return TmStatuses.Count - 1;
  }


  public int InitTmAnalog(int ch, int rtu, int point, string _ = null)
  {
    var existingTmAnalogIndex = FindTmAnalog(ch, rtu, point);
    if (existingTmAnalogIndex >= 0)
    {
      return existingTmAnalogIndex;
    }

    var tmAnalog = new TmAnalog(ch, rtu, point);

    TmAnalogs.Add(tmAnalog);
    
    tmAnalog.PropertyChanged += (_, _) => Update(updateElements: true);

    return TmAnalogs.Count - 1;
  }


  public TmStatus GetTmStatus(int idx)
  {
    return TmStatuses.ElementAtOrDefault(idx);
  }


  public int GetTmStatusStatus(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return -1;
    }
    return tmStatus.IsOn ? 1 : 0;
  }


  public string GetTmStatusCaption(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return "???";
    }
    return tmStatus.StatusCaption;
  }


  public bool IsTmStatusOn(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsOn;
  }


  public bool IsTmStatusUnreliable(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsUnreliable;
  }


  public bool IsTmStatusMalfunction(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsMalfunction;
  }


  public bool IsTmStatusIntermediate(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsIntermediate;
  }


  public bool IsTmStatusManuallySet(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsManuallySet;
  }


  public bool IsTmStatusManuallyBlocked(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsManuallyBlocked;
  }


  public bool IsTmStatusAbnormal(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsAbnormal;
  }


  public bool IsTmStatusUnacked(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsUnacked;
  }


  public bool IsTmStatusCustomFlagRaised(int idx, int level)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return level switch
           {
             1 => tmStatus.IsFlag1,
             2 => tmStatus.IsFlag2,
             3 => tmStatus.IsFlag3,
             4 => tmStatus.IsFlag4,
             _ => false
           };
  }


  public bool IsTmStatusUnderMaintenance(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsUnderMaintenance;
  }


  public bool IsTmStatusUnderCheckup(int idx)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return false;
    }
    return tmStatus.IsUnderCheckup;
  }


  public string GetTmStatusCustomFlagCaption(int idx, int level)
  {
    var tmStatus = TmStatuses.ElementAtOrDefault(idx);
    if (tmStatus == null)
    {
      return "???";
    }
    return level switch
           {
             1 => tmStatus.Flag1Status,
             2 => tmStatus.Flag2Status,
             3 => tmStatus.Flag3Status,
             4 => tmStatus.Flag4Status,
             _ => "???"
           };
  }


  public TmAnalog GetTmAnalog(int idx)
  {
    return TmAnalogs.ElementAtOrDefault(idx);
  }


  public bool IsTmAnalogUnreliable(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsUnreliable;
  }


  public bool IsTmAnalogManuallySet(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsManuallySet;
  }


  public bool IsTmAnalogManuallyBlocked(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsManuallyBlocked;
  }


  public bool IsTmAnalogAbnormal(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsAbnormal;
  }


  public bool IsTmAnalogUnacked(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsUnacked;
  }


  public bool IsTmAnalogAlarmAlert(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsAlarmLevel4;
  }


  public bool IsTmAnalogAlarmWarning(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return tmAnalog.IsAlarmLevel3;
  }


  public bool IsTmAnalogAlarmFlagRaised(int idx, int level)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return false;
    }
    return level switch
           {
             1 => tmAnalog.IsAlarmLevel1,
             2 => tmAnalog.IsAlarmLevel2,
             3 => tmAnalog.IsAlarmLevel3,
             4 => tmAnalog.IsAlarmLevel4,
             _ => false
           };
  }


  public float GetTmAnalogValue(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return 0;
    }
    return tmAnalog.Value;
  }


  public string GetTmAnalogValueString(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return TmAnalog.InvalidValueString;
    }
    return tmAnalog.ValueString;
  }


  public string GetTmAnalogUnit(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return string.Empty;
    }
    return tmAnalog.Unit;
  }


  public string GetTmAnalogValueWithUnitString(int idx)
  {
    var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
    if (tmAnalog == null)
    {
      return TmAnalog.InvalidValueString;
    }
    return tmAnalog.ValueWithUnitString;
  }


  public bool IsVariableUnreliable(int idx)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return false;
    }
    return variable.IsUnreliable;
  }


  public bool IsVariableManuallySet(int idx)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return false;
    }
    return variable.IsManuallySet;
  }


  public bool IsVariableOn(int idx)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return false;
    }
    return variable.IsOn;
  }


  public void SetVariable(int idx, bool isOn)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return;
    }
    variable.IsOn = isOn;
  }


  public void SetVariableUnreliable(int idx, bool isUnreliable)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return;
    }
    variable.IsUnreliable = isUnreliable;
  }


  public void SetVariableManuallySet(int idx, bool isManuallySet)
  {
    var variable = Variables.ElementAtOrDefault(idx);
    if (variable == null)
    {
      return;
    }
    variable.IsManuallySet = isManuallySet;
  }


  [RelayCommand]
  private void ClearLog()
  {
    Log.Clear();
  }


  public void AddToLog(string message, Element element = null)
  {
    if (element != null)
    {
      Log.Add(new LogItem($"{message}   {element}"));
    }
    else
    {
      Log.Add(new LogItem(message));
    }
  }


  public class LogItem
  {
    public DateTime Time    { get; }
    public string   Message { get; }


    public LogItem(string message)
    {
      Time    = DateTime.Now;
      Message = message;
    }
  }
}