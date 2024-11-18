var builder = WebApplication.CreateBuilder();
var app = builder.Build();

List<Order> orders =
[
    new(1, 3, 11, 2024, "Микроволновка", "не работает", "описание", "Егор", "не выполнено", "Андрей"),
    new(2, 5, 8, 2024, "Холодильник", "не работает", "описание", "Егор", "не выполнено", "Андрей"),
    new(3, 7, 3, 2024, "Духовка", "не работает", "описание", "Егор", "не выполнено", "Андрей"),
];


app.MapGet("/", () => orders);
app.MapPost("/", (Order o) => orders.Add(o));
app.MapPut("/{number}", (int number, OrderUpdateDTO dto) =>
{
    Order change = orders.Find(o => o.Number == number);
    if (change == null)
        return Results.NotFound("Не найдено!");
    if (change.Status != dto.Status)
    {
        change.Status = dto.Status;
    }
    if (change.Description != dto.Description)
    {
        change.Description = dto.Description;
    }
    if (change.Master != dto.Master)
    {
        change.Master = dto.Master;
    }
    return Results.Json(change);
});

app.Run();

record class OrderUpdateDTO(string Status, string Description, string Master);
class Order
{
    private int number;
    private int day;
    private int month;
    private int year;
    private string appliances;
    private string problemType;
    private string description;
    private string client;
    private string status;
    private string master;

    public Order(int number, int day, int month, int year, string appliances, string problemType, string description, string client, string status, string master)
    {
        Number = number;
        Day = day;
        Month = month;
        Year = year;
        Appliances = appliances;
        ProblemType = problemType;
        Description = description;
        Client = client;
        Status = status;
        Master = master;
    }

    public int Number { get => number; set => number = value; }
    public int Day { get => day; set => day = value; }
    public int Month { get => month; set => month = value; }
    public int Year { get => year; set => year = value; }
    public string Appliances { get => appliances; set => appliances = value; }
    public string ProblemType { get => problemType; set => problemType = value; }
    public string Description { get => description; set => description = value; }
    public string Client { get => client; set => client = value; }
    public string Status { get => status; set => status = value; }
    public string Master { get => master; set => master = value; }
}
