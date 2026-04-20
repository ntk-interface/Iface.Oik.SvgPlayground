using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Iface.Oik.SvgPlayground.Model;
using Iface.Oik.SvgPlayground.Util;
using Microsoft.Win32;
using SkiaSharp;
using Svg;

namespace Iface.Oik.SvgPlayground.MainWindow;

public partial class MainWindowViewModel : ObservableObject
{
  private const float ScaleStep = 1.5f;
  private const float MoveStep  = 50f;

  private readonly MainWindowView _view;

  private          string        _svgFilename;
  private          SvgDocument   _svgDocument;
  private readonly List<Element> _elements = new();

  [ObservableProperty]
  private string _title;

  private float _scale = 1;
  private float _x     = 0;
  private float _y     = 0;


  public ObservableCollection<TmStatus> TmStatuses { get; } = new();
  public ObservableCollection<TmAnalog> TmAnalogs  { get; } = new();
  public ObservableCollection<Variable> Variables  { get; } = new();
  

  public ObservableCollection<LogItem> Log { get; } = new();


  private int           _tickInterval = 1000;
  private PeriodicTimer _tickTimer;


  public MainWindowViewModel(MainWindowView view)
  {
    _view = view;

    Title = "SVG";

    FixTextBoxFloatValue();
  }


  private static void FixTextBoxFloatValue()
  {
    FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
  }


  [RelayCommand]
  private void OpenFile()
  {
    var openFileDialog = new OpenFileDialog
    {
      Filter = "SVG (*.svg)|*.svg",
    };
    if (openFileDialog.ShowDialog() != true)
    {
      return;
    }

    ExecuteOpenFile(openFileDialog.FileName);
  }


  [RelayCommand]
  private void ReloadFile()
  {
    ExecuteOpenFile(_svgFilename);
  }


  private void ExecuteOpenFile(string filename)
  {
    if (filename == null) return;

    _svgFilename = filename;
    try
    {
      ClearEverything();
      _svgDocument = SvgDocument.Open(filename);
      Title        = SvgUtil.FindTitleElement(_svgDocument)?.Content ?? "SVG";
      ParseElements();
      RunTickTimerIfNeeded();
      Update();
    }
    catch (Exception ex)
    {
      _svgDocument = null;
      MessageBox.Show("Ошибка при открытии файла: " + ex.Message);
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


  private void ParseElements()
  {
    _elements.Clear();

    try
    {
      var svgElements = SvgUtil.GetElementsCollectionWithAttribute(_svgDocument, "oikelement");
      foreach (var svgElement in svgElements)
      {
        var element = Element.Create(this, svgElement);
        if (element != null)
        {
          _elements.Add(element);
        }
      }
    }
    catch (Exception ex)
    {
      _elements.Clear();
      MessageBox.Show("Ошибка при разборе SVG, схема не будет оживлена: " + Environment.NewLine + ex.Message);
    }
  }


  private void Update()
  {
    if (_svgDocument == null) return;

    foreach (var element in _elements)
    {
      element.Update();
    }

    InvalidateCanvas();
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

    InvalidateCanvas();
  }


  private void InvalidateCanvas()
  {
    Application.Current
               .Dispatcher
              ?.Invoke(() => _view?.InvalidateCanvas());
  }


  public void OnCanvasPaintSurface(SKCanvas canvas)
  {
    try
    {
      SkiaSvgUtil.PaintSvgDocumentToSkiaCanvas(_svgDocument, canvas, _x, _y, _scale);
    }
    catch (Exception ex)
    {
      MessageBox.Show("Ошибка при попытке отрисовки: " + ex.Message);
    }
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

    tmStatus.PropertyChanged += (_, _) => Update();

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

    tmAnalog.PropertyChanged += (_, _) => Update();

    return TmAnalogs.Count - 1;
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
    switch (level)
    {
      case 1:  return tmStatus.IsFlag1;
      case 2:  return tmStatus.IsFlag2;
      case 3:  return tmStatus.IsFlag3;
      case 4:  return tmStatus.IsFlag4;
      default: return false;
    }
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
    switch (level)
    {
      case 1:  return tmAnalog.IsAlarmLevel1;
      case 2:  return tmAnalog.IsAlarmLevel2;
      case 3:  return tmAnalog.IsAlarmLevel3;
      case 4:  return tmAnalog.IsAlarmLevel4;
      default: return false;
    }
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


  public void AddToLog(object message)
  {
    Log.Add(new LogItem(message));
  }
    
    
  public class LogItem
  {
    public DateTime Time    { get; }
    public string   Message { get; }


    public LogItem(object message)
    {
      Time    = DateTime.Now;
      Message = message.ToString();
    }
  }
}