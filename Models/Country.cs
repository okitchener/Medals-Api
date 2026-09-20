public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Gold { get; set; } = 0;
    public int Silver { get; set; } = 0;
    public int Bronze { get; set; } = 0;
}
