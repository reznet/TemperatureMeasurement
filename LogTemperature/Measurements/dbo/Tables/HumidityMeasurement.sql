CREATE TABLE [dbo].[HumidityMeasurement]
(
	[HumidityMeasurementId] INT NOT NULL, 
    [HumidityPercentage] DECIMAL(18, 4) NOT NULL, 
    [MeasurementTimestamp] DATETIMEOFFSET NOT NULL, 
    [SourceName] NVARCHAR(256) NOT NULL,
	CONSTRAINT PK_HumidityMeasurement PRIMARY KEY (HumidityMeasurementId),
)
