CREATE TABLE [dbo].[TemperatureMeasurement] (
    [TemperatureMeasurementId] INT                IDENTITY (1, 1) NOT NULL,
    [TemperatureCelcius]       DECIMAL (18, 4)    NOT NULL,
    [MeasurementTimestamp]     DATETIMEOFFSET (7) NOT NULL
);

