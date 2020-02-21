namespace Iface.Oik.SvgPlayground.Model
{
  public class Variable
  {
    public string Id { get; set; }

    public bool IsOn          { get; set; }
    public bool IsUnreliable  { get; set; }
    public bool IsManuallySet { get; set; }


    public Variable(string id)
    {
      Id = id;
    }
  }
}