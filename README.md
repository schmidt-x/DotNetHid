# HidNet

![NuGet Version](https://img.shields.io/nuget/v/HidNet)
![GitHub License](https://img.shields.io/github/license/schmidt-x/HidNet)

.Net library for communicating with HID (Human Interface Devices). This library allows you to discover connected HID
devices (such as keyboards, mice, etc.) and establish bidirectional communication with them.

A great use of this tool is communication with programmable devices that use vendor-defined Usage Page ranges
(0xFF00-0xFFFF), enabling raw data exchange without interference from the operating system. One example is devices
powered by [QMK Firmware](https://docs.qmk.fm/) using the [Raw HID feature](https://docs.qmk.fm/features/rawhid).
Such functionality can be used, for instance, to send system metrics (e.g., CPU/GPU/RAM usage) to the keyboard for
display on its screen, or to remap input reports received from the keyboard to custom actions on the host system.

Usage:

```csharp
ushort vendorId  = 0x00; // device Vendor ID
ushort productId = 0x00; // device Product ID
ushort usagePage = 0x00; // HID Usage Page
ushort usageId   = 0x00; // HID Usage ID

var deviceInfo = Hid.Enumerate(vendorId, productId, usagePage, usageId).First();

Console.WriteLine($"Device name: {deviceInfo.ManufacturerString} - {deviceInfo.ProductString}");

var device = Hid.Create(deviceInfo);

if (!device.TryOpen(out var error))
{
  Console.WriteLine($"Failed to open: {error}");
  return;
}

using (device)
{
  if (!device.TryWrite([ 1, 2, 3, 4, 5 ], out error)) // sends data to the device
  {
    Console.WriteLine($"Failed to write: {error}");
    return;
  }

  var timeout = 1000; // ms

  var result = device.Read(timeout); // reads data from the device
  if (!result.IsOk(out var input, out error))
  {
    Console.WriteLine($"Failed to read: {error}");
    return;
  }

  Console.WriteLine($"[ {string.Join(", ", input)} ]");
}
```

## Caveats

- Both **reading** and **writing** methods **open** the device and **close** it afterward, **if it wasn't initially open.**
Therefore, it's not necessary to manually open the device for a single **read** or **write** operation.
- The `HidDevice` object is **not thread-safe** and should **not** be shared across threads. Create a new instance
(e.g., `var device = Hid.Create(...);`) for each thread.
- Calling `Dispose()` on an `HidDevice` instance (either explicitly or via a `using` block) **does not make** the object
unusable. Disposing is simply a shortcut for calling `Close()`, making it convenient to use with the `using` keyword for
automatic cleanup. The following two examples are functionally equivalent:
  ```csharp
  // device.Open...
  
  using (device)
  {
    // read/write operations...
  }
  
  // device can still be re-used here
  ```
  ```csharp
  // device.Open...
  
  try
  {
    // read/write operations...
  }
  finally
  {
    device.Close();
  }
  ```
- If the device gets disconnected during **read** or **write** operations (i.e., `error.Kind == ErrorKind.DeviceNotConnected`),
the device is automatically disposed. To re-establish communication, simply call `.Open` (or `.TryOpen`) on the same
device instance:
  ```csharp
  while (true) // assume we've opened the device earlier and are reading input reports in a loop
  {
    if (!device.Read().IsOk(out var input, out var error))
    {
      switch (error.Kind)
      {
      case ErrorKind.DeviceNotConnected: // device got disconnected
        if (device.TryOpen(out error)) // try to reconnect and continue
        {
          continue;
        }
        Console.WriteLine($"Failed to reconnect: {error}");
        return;
      
      // handle other error cases...
      }
    }
  }
  ```

## ToDo

- [ ] support older Target Frameworks
- [ ] support Linux
- [ ] support macOS
