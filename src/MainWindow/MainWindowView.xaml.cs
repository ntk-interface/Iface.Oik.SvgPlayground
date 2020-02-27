using SkiaSharp.Views.Desktop;

namespace Iface.Oik.SvgPlayground.MainWindow
{
  public partial class MainWindowView
  {
    private readonly MainWindowViewModel _viewModel;
    
    public MainWindowView()
    {
      InitializeComponent();
      
      _viewModel = new MainWindowViewModel(this);

      DataContext = _viewModel;
    }


    public void InvalidateCanvas()
    {
      Canvas.InvalidateVisual();
    }


    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
      _viewModel?.OnCanvasPaintSurface(e.Surface.Canvas);
    }
  }
}