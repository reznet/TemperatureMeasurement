//
//  main.cpp
//  LogTemperatureMac
//
//  Created by Jeff Evans on 12/24/14.
//  Copyright (c) 2014 Jeff Evans. All rights reserved.
//

#define TARGET_OS_MAC 1

#include <iostream>
#include <Carbon/Carbon.h>
#include "GoIO_DLL_interface.h"

char *deviceDesc[8] = {"?", "?", "Go! Temp", "Go! Link", "Go! Motion", "?", "?", "Mini GC"};

bool GetAvailableDeviceName(char *deviceName, gtype_int32 nameLength, gtype_int32 *pVendorId, gtype_int32 *pProductId)
{
    bool bFoundDevice = false;
    deviceName[0] = 0;
    int numSkips = GoIO_UpdateListOfAvailableDevices(VERNIER_DEFAULT_VENDOR_ID, SKIP_DEFAULT_PRODUCT_ID);
    int numJonahs = GoIO_UpdateListOfAvailableDevices(VERNIER_DEFAULT_VENDOR_ID, USB_DIRECT_TEMP_DEFAULT_PRODUCT_ID);
    int numCyclopses = GoIO_UpdateListOfAvailableDevices(VERNIER_DEFAULT_VENDOR_ID, CYCLOPS_DEFAULT_PRODUCT_ID);
    int numMiniGCs = GoIO_UpdateListOfAvailableDevices(VERNIER_DEFAULT_VENDOR_ID, MINI_GC_DEFAULT_PRODUCT_ID);
    
    if (numSkips > 0)
    {
        GoIO_GetNthAvailableDeviceName(deviceName, nameLength, VERNIER_DEFAULT_VENDOR_ID, SKIP_DEFAULT_PRODUCT_ID, 0);
        *pVendorId = VERNIER_DEFAULT_VENDOR_ID;
        *pProductId = SKIP_DEFAULT_PRODUCT_ID;
        bFoundDevice = true;
    }
    else if (numJonahs > 0)
    {
        GoIO_GetNthAvailableDeviceName(deviceName, nameLength, VERNIER_DEFAULT_VENDOR_ID, USB_DIRECT_TEMP_DEFAULT_PRODUCT_ID, 0);
        *pVendorId = VERNIER_DEFAULT_VENDOR_ID;
        *pProductId = USB_DIRECT_TEMP_DEFAULT_PRODUCT_ID;
        bFoundDevice = true;
    }
    else if (numCyclopses > 0)
    {
        GoIO_GetNthAvailableDeviceName(deviceName, nameLength, VERNIER_DEFAULT_VENDOR_ID, CYCLOPS_DEFAULT_PRODUCT_ID, 0);
        *pVendorId = VERNIER_DEFAULT_VENDOR_ID;
        *pProductId = CYCLOPS_DEFAULT_PRODUCT_ID;
        bFoundDevice = true;
    }
    else if (numMiniGCs > 0)
    {
        GoIO_GetNthAvailableDeviceName(deviceName, nameLength, VERNIER_DEFAULT_VENDOR_ID, MINI_GC_DEFAULT_PRODUCT_ID, 0);
        *pVendorId = VERNIER_DEFAULT_VENDOR_ID;
        *pProductId = MINI_GC_DEFAULT_PRODUCT_ID;
        bFoundDevice = true;
    }
    
    return bFoundDevice;
}

int main(int argc, const char * argv[]) {
    GoIO_Init();
    
    gtype_uint16 MajorVersion;
    gtype_uint16 MinorVersion;
    GoIO_GetDLLVersion(&MajorVersion, &MinorVersion);
    printf("This app is linked to GoIO lib version %d.%d .\n", MajorVersion, MinorVersion);
    
    gtype_int32 vendorId;		//USB vendor id
    gtype_int32 productId;		//USB product id
    char deviceName[GOIO_MAX_SIZE_DEVICE_NAME];
    bool bFoundDevice = GetAvailableDeviceName(deviceName, GOIO_MAX_SIZE_DEVICE_NAME, &vendorId, &productId);
    if (!bFoundDevice) {
        printf("No Go devices found.\n");
    } else {
        GOIO_SENSOR_HANDLE hDevice = GoIO_Sensor_Open(deviceName, vendorId, productId, 0);
        if (hDevice != NULL)
        {
            printf("Successfully opened %s device %s .\n", deviceDesc[productId], deviceName);
            
            GoIO_Sensor_SetMeasurementPeriod(hDevice, 1, SKIP_TIMEOUT_MS_DEFAULT);
            
            GoIO_Sensor_SendCmdAndGetResponse(hDevice, SKIP_CMD_ID_START_MEASUREMENTS, nullptr, 0, nullptr, nullptr, SKIP_TIMEOUT_MS_DEFAULT);
            
            // sleep 1 second
            sleep(1);
            
            const int maxMeasurementCount = 100;
            gtype_int32 rawMeasurements[maxMeasurementCount];
            gtype_int32 measurementsRead = GoIO_Sensor_ReadRawMeasurements(hDevice, rawMeasurements, maxMeasurementCount);
            
            GoIO_Sensor_SendCmdAndGetResponse(hDevice, SKIP_CMD_ID_STOP_MEASUREMENTS, nullptr, 0, nullptr, nullptr, SKIP_TIMEOUT_MS_DEFAULT);
            
            gtype_real64 voltages[maxMeasurementCount];
            gtype_real64 temperatures[maxMeasurementCount];
            gtype_real64 averageTemperature = 0;
            
            for(int i = 0; i < measurementsRead; i++){
                voltages[i] = GoIO_Sensor_ConvertToVoltage(hDevice, rawMeasurements[i]);
                temperatures[i] = GoIO_Sensor_CalibrateData(hDevice, voltages[i]);
                averageTemperature += temperatures[i];
            }
            
            if(measurementsRead > 0){
                averageTemperature /= measurementsRead;
            }
            
            printf("Average measurement: %8.3f\n", averageTemperature);
            
            
            
            GoIO_Sensor_Close(hDevice);
        }
    }
    
    GoIO_Uninit();
    return 0;
}
