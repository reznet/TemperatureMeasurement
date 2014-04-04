CREATE PROC [dbo].[GetHumidityMeasurements]
AS
BEGIN
	SELECT h.HumidityMeasurementId, h.HumidityPercentage, h.MeasurementTimestamp, h.SourceName
	FROM dbo.HumidityMeasurement AS h
	WHERE MeasurementTimestamp > DATEADD(Day, -1, SYSDATETIMEOFFSET())
	ORDER BY h.HumidityMeasurementId
END