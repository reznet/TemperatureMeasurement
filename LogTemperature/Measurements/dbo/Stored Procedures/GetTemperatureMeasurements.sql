CREATE PROC [dbo].[GetTemperatureMeasurements]
AS
BEGIN
	SELECT *, 32 + (1.8 * TemperatureCelcius) AS TemperatureFahrenheit
	FROM dbo.TemperatureMeasurement
	WHERE MeasurementTimestamp > DATEADD(Day, -1, SYSDATETIMEOFFSET())
	ORDER BY TemperatureMeasurementId DESC
END