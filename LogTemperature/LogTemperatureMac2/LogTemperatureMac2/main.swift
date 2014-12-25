//
//  main.swift
//  LogTemperatureMac2
//
//  Created by Jeff Evans on 12/24/14.
//  Copyright (c) 2014 Jeff Evans. All rights reserved.
//

import Foundation

println("Hello, World!")

var majorVersion : gtype_uint16 = 0
var minorVersion : gtype_uint16 = 0

GoIO_GetDLLVersion(&majorVersion, &minorVersion)

println("This app is linked to GoIO lib version \(majorVersion).\(minorVersion)")