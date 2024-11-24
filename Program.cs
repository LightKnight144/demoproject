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
    new(1, new(2023, 11, 3), "Микроволновка", "не работает", "описание", "Егор", "в ожидании"),
    new(2, new(2023, 8, 5), "Холодильник", "не работает", "описание", "Егор", "в ожидании"),
    new(3, new(2023, 3, 7), "Духовка", "не работает", "описание", "Егор", "в ожидании"),
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
    if (dto.Status != change.Status)
    {
        change.Status = dto.Status;
        message += "Изменён статус заявки под номером " + change.Number + "\n";
        if (dto.Status == "выполнено")
        {
            message += "Заявка номер " + change.Number + " завершена" + "\n";
        }
    }
    if (dto.Description != "")
    {
        change.Description = dto.Description;
    }
    if (dto.Master != "")
    {
        change.Master = dto.Master;
    }
    if (dto.Comments != "")
    {
        change.Comments.Add(dto.Comments);
    }
});

int complete_count() => orders.FindAll(o => o.Status == "выполнено").Count;

Dictionary<string, int> problemType_stat() =>
    orders.GroupBy(o => o.ProblemType)
    .Select(o => (o.Key, o.Count()))
    .ToDictionary(k => k.Key, i => i.Item2);

double averageTime() =>
    complete_count() == 0 ? 0 :
    orders.FindAll(o => o.Status == "выполнено")
    .Select(o => o.EndDate.Value.DayNumber - o.StartDate.DayNumber)
    .Sum() / complete_count();

app.MapGet("/statistic", () =>
new
{
    complete_count = complete_count(),
    problemType_stat = problemType_stat(),
    averageTime = averageTime()
});


app.Run();

record class OrderUpdateDTO(int Number, string Status, string Description, string Master, string Comments);
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
    public List<string> Comments { get; set; } = [];
}
