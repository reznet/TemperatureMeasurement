//
//  main.swift
//  LogTemperatureMac2
//
//  Created by Jeff Evans on 12/24/14.
//  Copyright (c) 2014 Jeff Evans. All rights reserved.
//

import Foundation

let temperatureSourceName = "Office"

GoIO_Init()

var majorVersion : gtype_uint16 = 0
var minorVersion : gtype_uint16 = 0

GoIO_GetDLLVersion(&majorVersion, &minorVersion)

println("This app is linked to GoIO lib version \(majorVersion).\(minorVersion)")

let vendorId = gtype_int32(VERNIER_DEFAULT_VENDOR_ID)
let productId = gtype_int32(USB_DIRECT_TEMP_DEFAULT_PRODUCT_ID)

let numberOfDevices = GoIO_UpdateListOfAvailableDevices(vendorId, productId)

println("There are \(numberOfDevices) device(s)")

var deviceNameArray : Array<CChar> = Array<CChar>(count: 255, repeatedValue: 0)
GoIO_GetNthAvailableDeviceName(&deviceNameArray, gtype_int32(255), vendorId, productId, 0)

let deviceName = String.fromCString(deviceNameArray)!

println("opening sensor device \(deviceName)")
var device = GoIO_Sensor_Open(deviceName, vendorId, productId, 0)

if(device != nil){

    GoIO_Sensor_SetMeasurementPeriod(device, 1, SKIP_TIMEOUT_MS_DEFAULT)

    GoIO_Sensor_SendCmdAndGetResponse(device, UInt8(SKIP_CMD_ID_START_MEASUREMENTS), nil, 0, nil, nil, SKIP_TIMEOUT_MS_DEFAULT)

    // sleep 1 second
    sleep(1)

    let maxMeasurementCount = 100
    var rawMeasurements : Array<gtype_int32> = Array<gtype_int32>(count: maxMeasurementCount, repeatedValue: 0)
    let readCount = GoIO_Sensor_ReadRawMeasurements(device, &rawMeasurements, gtype_int32(maxMeasurementCount))

    GoIO_Sensor_SendCmdAndGetResponse(device, UInt8(SKIP_CMD_ID_STOP_MEASUREMENTS), nil, 0, nil, nil, SKIP_TIMEOUT_MS_DEFAULT)

    var voltages : Array<gtype_real64> = Array<gtype_real64>(count: maxMeasurementCount, repeatedValue: 0)
    var temperatures : Array<gtype_real64> = Array<gtype_real64>(count: maxMeasurementCount, repeatedValue: 0)
    var averageTemperature = gtype_real64(0)

    for i in 0...Int(readCount) {
        voltages[i] = GoIO_Sensor_ConvertToVoltage(device, rawMeasurements[i])
        temperatures[i] = GoIO_Sensor_CalibrateData(device, voltages[i])
        averageTemperature += temperatures[i]
    }

    if(readCount > 0){
        averageTemperature /= gtype_real64(readCount)
    }

    println("Average temperature: \(averageTemperature)")
    
    let url = NSURL(string: "http://temperatures.azurewebsites.net/api/temperature/new")
    var request = NSMutableURLRequest(URL: url!)
    request.HTTPMethod = "POST"
    let body = NSString(format: "{ \"TemperatureCelcius\" : \(averageTemperature), \"Source\" : \"\(temperatureSourceName)\" }")
    request.HTTPBody = body.dataUsingEncoding(NSUTF8StringEncoding)
    request.addValue("application/json", forHTTPHeaderField: "Content-Type")
    
    var response : NSURLResponse?
    var error : NSError?
    
    println("sending \(body) to \(url!)")
    let responseData = NSURLConnection.sendSynchronousRequest(request, returningResponse: &response, error: &error)
    
    let responseString = NSString(data: responseData!, encoding: NSUTF8StringEncoding)!
    
    if let httpResponse  = response! as? NSHTTPURLResponse {
        println("HTTP status code \(httpResponse.statusCode)")
    }

    println("closing sensor")
    GoIO_Sensor_Close(device)
}

GoIO_Uninit()