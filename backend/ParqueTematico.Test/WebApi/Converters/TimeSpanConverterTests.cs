using System.Text.Json;
using ParqueTematico.WebApi.Converters;

namespace Test.WebApi.Converters;

[TestClass]
public class TimeSpanConverterTests
{
    [TestMethod]
    public void Read_DebeConvertirStringATimeSpan()
    {
        var json = "\"20:00:00\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimeSpanConverter());

        TimeSpan result = JsonSerializer.Deserialize<TimeSpan>(json, options);

        Assert.AreEqual(new TimeSpan(20, 0, 0), result);
    }

    [TestMethod]
    public void Write_DebeConvertirTimeSpanAString()
    {
        var timeSpan = new TimeSpan(20, 0, 0);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimeSpanConverter());

        var json = JsonSerializer.Serialize(timeSpan, options);

        Assert.AreEqual("\"20:00:00\"", json);
    }

    [TestMethod]
    public void Read_DebeConvertirDiferentesFormatos()
    {
        var json = "\"14:30:45\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimeSpanConverter());

        TimeSpan result = JsonSerializer.Deserialize<TimeSpan>(json, options);

        Assert.AreEqual(new TimeSpan(14, 30, 45), result);
    }
}
