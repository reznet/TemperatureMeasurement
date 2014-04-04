CREATE PROC [dbo].[LogHumidityMeasurement]
	@HumidityPercentage DECIMAL(18,4),
	@SourceName NVARCHAR(256)
AS
BEGIN
	INSERT INTO dbo.HumidityMeasurement(HumidityPercentage, MeasurementTimestamp, SourceName)
	VALUES (@HumidityPercentage, SYSDATETIMEOFFSET(), @SourceName)

	RETURN SCOPE_IDENTITY();
END