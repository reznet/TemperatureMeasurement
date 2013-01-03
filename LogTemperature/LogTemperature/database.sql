CREATE PROC dbo.LogTemperatureMeasurement
	@TemperatureCelcius DECIMAL(18,4)
AS
BEGIN
	INSERT INTO dbo.TemperatureMeasurement(TemperatureCelcius, MeasurementTimestamp)
	VALUES (@TemperatureCelcius, SYSDATETIMEOFFSET())

	RETURN SCOPE_IDENTITY();
END