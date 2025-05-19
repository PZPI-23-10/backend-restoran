namespace backend_restoran.Persistence.Models;

[Serializable]
public class RestaurantLayout
{
  public List<LayoutItem> Layout { get; set; } = [];
}

[Serializable]
public class LayoutItem
{
  public int Id { get; set; }
  public Position Position { get; set; }
  public LayoutItemType Type { get; set; }
}

[Serializable]
public class Position
{
  public int X { get; set; }
  public int Y { get; set; }
}

[Serializable]
public enum LayoutItemType
{
  StraightWall,
  CircleWall,
  TriangleWall, 
  Door,
  Window,
  DoubleSit,
  MultipleSit,
  ComfortableSit,
  Bar,
  Lighting,
  Stairs
}