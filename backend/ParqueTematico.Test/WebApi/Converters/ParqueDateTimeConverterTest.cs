using System.Text.Json;
using ParqueTematico.WebApi.Converters;

namespace Test.WebApi.Converters;

[TestClass]
public class ParqueDateTimeConverterTest
{
    private JsonSerializerOptions _options = null!;

    [TestInitialize]
    public void Setup()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new ParqueDateTimeConverter());
    }

    [TestMethod]
    public void Serialize_DateTime_FormatoCorrecto()
    {
        var date = new DateTime(2025, 9, 2, 14, 45, 0);
        var json = JsonSerializer.Serialize(date, _options);
        Assert.AreEqual("\"2025-09-02T14:45\"", json);
    }

    [TestMethod]
    public void Deserialize_StringValido_DevuelveDateTimeCorrecto()
    {
        var json = "\"2025-09-02T14:45\"";
        DateTime result = JsonSerializer.Deserialize<DateTime>(json, _options);
        Assert.AreEqual(new DateTime(2025, 9, 2, 14, 45, 0), result);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void Deserialize_FormatoInvalido_LanzaExcepcion()
    {
        var json = "\"02-09-2025 14:45\"";
        _ = JsonSerializer.Deserialize<DateTime>(json, _options);
    }
}
