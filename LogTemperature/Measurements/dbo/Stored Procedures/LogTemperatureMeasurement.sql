CREATE PROC [dbo].[LogTemperatureMeasurement]
	@TemperatureCelcius DECIMAL(18,4),
	@SourceName NVARCHAR(256) = 'Living Room'
AS
BEGIN
	INSERT INTO dbo.TemperatureMeasurement(TemperatureCelcius, MeasurementTimestamp, SourceName)
	VALUES (@TemperatureCelcius, SYSDATETIMEOFFSET(), @SourceName)

	RETURN SCOPE_IDENTITY();
END