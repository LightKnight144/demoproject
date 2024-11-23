var builder = WebApplication.CreateBuilder();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost:5164", builder => builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});


var app = builder.Build();

app.UseCors("AllowLocalhost:5164");

List<Order> orders =
[
    new(1, new(2024, 11, 3), "Микроволновка", "не работает", "описание", "Егор", "в ожидании"),
    new(2, new(2024, 8, 5), "Холодильник", "не работает", "описание", "Егор", "в ожидании"),
    new(3, new(2024, 3, 7), "Духовка", "не работает", "описание", "Егор", "в ожидании"),
];

string message = "";

app.MapGet("/orders", (int param = 0) =>
{
    string buffer = message;
    message = "";
    if (param != 0)
        return new { orders = orders.FindAll(o => o.Number == param), message = buffer };
    return new { orders, message = buffer };
});
app.MapGet("/create", ([AsParameters] Order o) => orders.Add(o));
app.MapGet("/update", ([AsParameters] OrderUpdateDTO dto) =>
{
    var change = orders.Find(o => o.Number == dto.Number);
    if (change == null)
        return;
    if (dto.Status != change.Status && dto.Status != "")
    {
        change.Status = dto.Status;
        message += "Изменён статус заявки под номером" + change.Number + "\n";
    }
    if (dto.Description != "")
    {
        change.Description = dto.Description;
    }
    if (dto.Master != "")
    {
        change.Master = dto.Master;
    }
});

app.Run();

record class OrderUpdateDTO(int Number, string Status, string Description, string Master);
class Order(int number, DateOnly startDate, string appliances, string problemType, string description, string client, string status)
{

    public int Number { get; set; } = number;
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly? EndDate { get; set; } = null;
    public string Appliances { get; set; } = appliances;
    public string ProblemType { get; set; } = problemType;
    public string Description { get; set; } = description;
    public string Client { get; set; } = client;
    public string Status { get; set; } = status;
    public string Master { get; set; } = "Не назначено";
}
