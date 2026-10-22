namespace UnionTypes.Api.Models;

// A sensor reports a number, or a status text when it has no value.
// int and string use different JSON token kinds, so no classifier is needed.
public union Reading(int, string);

public record class SensorReading(string Sensor, Reading Value);
